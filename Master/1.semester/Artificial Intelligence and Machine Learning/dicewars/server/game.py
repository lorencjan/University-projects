import json
from json.decoder import JSONDecodeError
import logging
import random
import numpy as np
import socket
import sys

from .player import Player

from .summary import GameSummary


class Game:
    """Instance of the game
    """
    def __init__(self, board, area_ownership, players, game_config, addr, port, nicknames_order):
        """Initialize game and connect clients

        Parameters
        ----------
        players : int
            Number of players
        addr : str
            IP address of the server
        port : int
            Port number

        Attributes
        ----------
        buffer : int
            Size of socket buffer
        number_of_players : int
            Number of players
        """
        self.buffer = 65535
        self.logger = logging.getLogger('SERVER')

        self.address = addr
        self.port = port
        self.number_of_players = players

        self.nb_players_alive = players
        self.nb_consecutive_end_of_turns = 0
        self.nb_battles = 0

        self.reserve_production_cap = game_config.getint('ReserveProductionCap')
        self.reserve_type = game_config.get('ReserveType')
        self.reserve_cap = game_config.getint('ReserveSizeCap')
        self.max_dice_per_area = game_config.getint('MaxDicePerArea')
        self.max_pass_rounds = game_config.getint('MaximumNoBattleRounds')
        self.max_battles_per_game = game_config.getint('MaximumBattlesPerGame')
        self.battle_wear_min = game_config.getint('BattleWearMinimum')

        deployment_method = game_config['DeploymentMethod']
        if deployment_method == 'unlimited':
            self.max_deployed_dice = UnlimitedDeployment(self.max_dice_per_area)
        elif deployment_method == 'limited':
            self.max_deployed_dice = LimitedDeployment(self.max_dice_per_area)
        else:
            raise ValueError(f'Unknown deployement method "{deployment_method}"')

        self.create_socket()

        self.board = board
        self.initialize_players()

        self.connect_clients()
        if nicknames_order is not None:
            self.adjust_player_order(nicknames_order)
        self.report_player_order()

        self.assign_areas_to_players(area_ownership)
        self.logger.debug("Board initialized")

        for player in self.players.values():
            self.send_message(player, 'game_start')

        self.summary = GameSummary()

    def run(self):
        """Main loop of the game
        """
        try:
            for i in range(1, self.number_of_players + 1):
                player = self.players[i]
                self.send_message(player, 'game_state')
            while True:
                self.logger.debug("Current player {}".format(self.current_player.get_name()))
                self.handle_player_turn()
                if self.check_win_condition():
                    sys.stdout.write(str(self.summary))
                    break

        except KeyboardInterrupt:
            self.logger.info("Game interrupted.")
            for i in range(1, self.number_of_players + 1):
                player = self.players[i]
                self.send_message(player, 'close_socket')
        except BrokenPipeError as e:
            self.logger.error("Connection to client failed: {0}".format(e), exc_info=True)
        except JSONDecodeError as e:
            self.logger.error("Failed to parse the client message: {0}".format(e), exc_info=True)
        except ConnectionResetError:
            self.logger.error("ConnectionResetError", exc_info=True)

        try:
            self.close_connections()
        except BrokenPipeError:
            pass

    ##############
    # GAME LOGIC #
    ##############
    def assign_area(self, area, player):
        """Assign area to a new owner

        Parameters
        ----------
        area : Area
            Area to be assigned new owner to
        player : Player
            New owner
        """
        area.set_owner_name(player.get_name())
        player.add_area(area)

    def handle_player_turn(self):
        """Handle clients message and carry out the action
        """
        self.logger.debug("Handling player {} ({}) turn".format(self.current_player.get_name(), self.current_player.nickname))
        player = self.current_player.get_name()
        msg = self.get_message(player)

        if msg['type'] == 'battle':
            self.nb_consecutive_end_of_turns = 0
            battle = self.battle(self.board.get_area_by_name(msg['atk']), self.board.get_area_by_name(msg['def']))
            self.summary.add_battle()
            self.logger.debug("Battle result: {}".format(battle))
            for p in self.players:
                self.send_message(self.players[p], 'battle', battle=battle)

        elif msg['type'] == 'end_turn':
            self.nb_consecutive_end_of_turns += 1
            affected_areas = self.end_turn()
            for p in self.players:
                self.send_message(self.players[p], 'end_turn', areas=affected_areas)

        elif msg['type'] == 'transfer':
            self.nb_consecutive_end_of_turns = 0
            transfer = self.transfer(self.board.get_area_by_name(msg['src']), self.board.get_area_by_name(msg['dst']))
            for p in self.players:
                self.send_message(self.players[p], 'transfer', transfer=transfer)

        else:
            self.logger.warning(f'Unexpected message type: {msg["type"]}')

    def get_state(self):
        """Get game state

        Returns
        -------
        dict
            Dictionary containing owner, dice and adjacent areas of
            each area, as well as score of each player
        """
        game_state = {
            'areas': {}
        }

        for a in self.board.areas:
            area = self.board.areas[a]
            game_state['areas'][area.name] = {
                'adjacent_areas': area.get_adjacent_areas_names(),
                'owner': area.get_owner_name(),
                'dice': area.get_dice()
            }

        game_state['score'] = {}

        for p in self.players:
            player = self.players[p]
            game_state['score'][player.get_name()] = player.get_largest_region(self.board)

        return game_state

    def battle(self, attacker, defender):
        """Carry out a battle

        Returns
        -------
        dict
            Dictionary with the result of the battle including information
            about rolled numbers, dice left after the battle, and possible
            new ownership of the areas
        """
        self.nb_battles += 1
        atk_dice = attacker.get_dice()
        def_dice = defender.get_dice()
        atk_pwr = def_pwr = 0

        atk_name = attacker.get_owner_name()
        def_name = defender.get_owner_name()

        for i in range(0, atk_dice):
            atk_pwr += random.randint(1, 6)
        for i in range(0, def_dice):
            def_pwr += random.randint(1, 6)

        battle = {
            'atk': {
                'name': attacker.get_name(),
                'dice': 1,
                'owner': atk_name,
                'pwr': atk_pwr
            }
        }

        attacker.set_dice(1)

        if atk_pwr > def_pwr:
            defender.set_owner_name(atk_name)
            self.players[atk_name].add_area(defender)
            self.players[def_name].remove_area(defender)
            if self.players[def_name].get_number_of_areas() == 0:
                self.eliminate_player(def_name)

            attacker.set_dice(1)
            defender.set_dice(atk_dice - 1)
            battle['def'] = {
                'name': defender.get_name(),
                'dice': atk_dice - 1,
                'owner': atk_name,
                'pwr': def_pwr
            }

        else:
            battle_wear = atk_dice // self.battle_wear_min
            def_dice_left = max(1, def_dice - battle_wear)
            defender.set_dice(def_dice_left)
            battle['def'] = {
                'name': defender.get_name(),
                'dice': def_dice_left,
                'owner': def_name,
                'pwr': def_pwr
            }

        return battle

    def transfer(self, source, destination):
        """Carry out a transfer

        Returns
        -------
        dict
            Dictionary with the result of the transfer
        """
        src_dice = source.get_dice()
        dst_dice = destination.get_dice()

        dice_moved = min(self.max_dice_per_area - dst_dice, src_dice - 1)

        source.set_dice(src_dice - dice_moved)
        destination.set_dice(dst_dice + dice_moved)

        transfer = {
            'src': {
                'name': source.get_name(),
                'dice': source.get_dice(),
            },
            'dst': {
                'name': destination.get_name(),
                'dice': destination.get_dice(),
            }
        }

        return transfer

    def end_turn(self):
        """Handles end turn command

        Returns
        -------
        dict
            Dictionary of affected areas including number of dice in these areas
        """

        deployable_dice, reserve_dice = self.get_player_dice(self.current_player)
        affected_areas = self.distribute_player_dice(self.current_player, deployable_dice)

        if self.reserve_type == 'constant':
            if reserve_dice > self.reserve_cap:
                reserve_dice = self.reserve_cap
        elif self.reserve_type == 'complement':
            reserve_cap = self.reserve_cap - len(self.current_player.get_areas())
            if reserve_dice > reserve_cap:
                reserve_dice = reserve_cap
        else:
            raise ValueError(f'Unsupported reserve type: {self.reserve_type}')

        if reserve_dice < 0:
            reserve_dice = 0

        self.current_player.set_reserve(reserve_dice)

        self.set_next_player()

        list_of_areas = {}
        for area in affected_areas:
            list_of_areas[area.get_name()] = {
                'owner': area.get_owner_name(),
                'dice': area.get_dice()
            }

        return list_of_areas

    def get_player_dice(self, player):
        free_dice = player.get_reserve() + player.get_largest_region(self.board)
        if free_dice > self.reserve_production_cap:
            free_dice = self.reserve_production_cap

        dice_deployed = sum(a.get_dice() for a in player.get_areas())
        max_deployed = self.max_deployed_dice(player)
        room_for_deployment = max(max_deployed - dice_deployed, 0)
        available_for_deployment = min(free_dice, room_for_deployment)
        free_dice = max(0, free_dice - available_for_deployment)

        return available_for_deployment, free_dice

    def distribute_player_dice(self, player, available_for_deployment):
        areas = []
        for area in self.current_player.get_areas():
            areas.append(area)

        affected_areas = []
        while available_for_deployment and areas:
            area = random.choice(areas)
            if area.get_dice() >= self.max_dice_per_area:
                areas.remove(area)
            else:
                if area not in affected_areas:
                    affected_areas.append(area)
                area.dice += 1
                available_for_deployment -= 1

        return affected_areas

    def set_first_player(self):
        """Set first player
        """
        for player in self.players:
            if self.players[player].get_name() == self.players_order[0]:
                self.current_player = self.players[player]
                self.logger.debug("Current player: {}".format(self.current_player.get_name()))
                return

    def set_next_player(self):
        """Set next player in order as a current player
        """
        current_player_name = self.current_player.get_name()
        current_idx = self.players_order.index(current_player_name)
        idx = self.players_order[(current_idx + 1) % self.number_of_players]
        while True:
            try:
                if self.players[idx].get_number_of_areas() == 0:
                    current_idx = (current_idx + 1) % self.number_of_players
                    idx = self.players_order[(current_idx + 1) % self.number_of_players]
                    continue
                self.current_player = self.players[idx]
                self.logger.debug("Current player: {}".format(self.current_player.get_name()))
            except IndexError:
                exit(1)
            return

    def eliminate_player(self, player):
        nickname = self.players[player].get_nickname()
        self.summary.add_elimination(nickname, self.summary.nb_battles)
        self.logger.info("Eliminated player {} ({})".format(player, nickname))
        self.nb_players_alive -= 1

    def check_win_condition(self):
        """Check win conditions

        Returns
        -------
        bool
            True if a player has won, False otherwise
        """
        if self.nb_consecutive_end_of_turns // self.nb_players_alive == self.max_pass_rounds:
            self.logger.info("Game cancelled because the limit of {} rounds of passing has been reached".format(self.max_pass_rounds))
            for p in self.players.values():
                if p.get_number_of_areas() > 0:
                    self.eliminate_player(p.get_name())

            self.process_win(None, -1)
            return True

        if self.nb_battles == self.max_battles_per_game:
            self.logger.info("Game cancelled because the limit of {} battles has been reached".format(self.max_battles_per_game))
            for p in self.players.values():
                if p.get_number_of_areas() > 0:
                    self.eliminate_player(p.get_name())

            self.process_win(None, -1)
            return True

        for p in self.players:
            player = self.players[p]
            if player.get_number_of_areas() == self.board.get_number_of_areas():
                self.process_win(player.get_nickname(), player.get_name())
                return True

        return False

    def process_win(self, player_nick, player_name):
        self.summary.set_winner(player_nick)
        self.logger.info("Player {} ({}) wins!".format(player_nick, player_name))
        for i in self.players:
            self.send_message(self.players[i], 'game_end', winner=player_name)

    ##############
    # NETWORKING #
    ##############
    def get_message(self, player):
        """Read message from client

        Parameters
        ----------
        player : int
            Name of the client

        Returns
        -------
        str
            Decoded message from the client
        """
        raw_message = self.client_sockets[player].recv(self.buffer)
        msg = json.loads(raw_message.decode())
        self.logger.debug("Got message from client {}: {}".format(player, msg))
        return msg

    def send_message(self, client, type, battle=None, winner=None, areas=None, transfer=None):
        """Send message to a client

        Parameters
        ----------
        client : Player
            Recepient of the message
        type : str
            Type of message
        battle : dict
            Result of a battle
        winner : int
            Winner of the game
        areas : list of int
            Areas changed during the turn
        """
        self.logger.debug("Sending msg type '{}' to client {}".format(type, client.get_name()))
        if type == 'game_start':
            msg = self.get_state()
            msg['type'] = 'game_start'
            msg['player'] = client.get_name()
            msg['no_players'] = self.number_of_players
            msg['current_player'] = self.current_player.get_name()
            msg['board'] = self.board.get_board()
            msg['order'] = self.players_order

        elif type == 'game_state':
            msg = self.get_state()
            msg['type'] = 'game_state'
            msg['player'] = client.get_name()
            msg['no_players'] = self.number_of_players
            msg['current_player'] = self.current_player.get_name()

        elif type == 'battle':
            msg = self.get_state()
            msg['type'] = 'battle'
            msg['result'] = battle

        elif type == 'transfer':
            msg = self.get_state()
            msg['type'] = 'transfer'
            msg['result'] = transfer

        elif type == 'end_turn':
            msg = self.get_state()
            msg['type'] = 'end_turn'
            msg['areas'] = areas
            msg['current_player'] = self.current_player.get_name()
            msg['reserves'] = {
                i: self.players[i].get_reserve() for i in self.players
            }

        elif type == 'game_end':
            msg = {
                'type': 'game_end',
                'winner': winner
            }

        elif type == 'close_socket':
            msg = {'type': 'close_socket'}

        msg = json.dumps(msg)
        client.send_message(msg + '\0')

    def create_socket(self):
        """Initiate server socket
        """
        try:
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
            self.socket.bind((self.address, self.port))
            self.logger.debug("Server socket at {}:{}".format(self.address, self.port))
        except OSError as e:
            self.logger.error("Cannot create socket. {0}.".format(e))
            exit(1)

    def connect_clients(self):
        """Connect all clients
        """
        self.client_sockets = {}

        self.socket.listen(self.number_of_players)
        self.logger.debug("Waiting for clients to connect")

        for i in range(1, self.number_of_players + 1):
            self.connect_client(i)
            hello_msg = self.get_message(i)
            if hello_msg['type'] != 'client_desc':
                raise ValueError("Client send a wrong-type hello message '{}'".format(hello_msg))
            self.players[i].set_nickname(hello_msg['nickname'])

        self.logger.debug("Successfully assigned clients to all players")

    def connect_client(self, i):
        """Assign client to an instance of Player
        """
        sock, client_address = self.socket.accept()
        self.add_client(sock, client_address, i)

    def add_client(self, connection, client_address, i):
        """Add client's socket to an instance of Player

        Parameters
        ----------
        connection : socket
            Client's socket
        client_addres : (str, int)
            Client's address and port number
        i : int
            Player's name

        Returns
        -------
        Player
            Instance of Player that the client was assigned to
        """
        self.client_sockets[i] = connection
        player = self.assign_player_to_client(connection, client_address)
        if not player:
            raise Exception("Could not assign player to client {}".format(client_address))
        else:
            return player

    def assign_player_to_client(self, socket, client_address):
        """Add client's socket to an unassigned player
        """
        player = self.get_unassigned_player()
        if player:
            player.assign_client(socket, client_address)
            return player
        else:
            return False

    def get_unassigned_player(self):
        """Get a player with unassigned client
        """
        for player in self.players:
            if not self.players[player].has_client():
                return self.players[player]
        return False

    def close_connections(self):
        """Close server's socket
        """
        self.logger.debug("Closing server socket")
        self.socket.close()

    ##################
    # INITIALIZATION #
    ##################
    def initialize_players(self):
        self.players = {}
        for i in range(1, self.number_of_players + 1):
            self.players[i] = Player(i)

        self.players_order = list(range(1, self.number_of_players + 1))
        random.shuffle(self.players_order)

        self.set_first_player()
        self.logger.debug("Player order {0}".format(self.players_order))

    def assign_areas_to_players(self, ownership):
        """Assigns areas to players at the start of the game
        """

        assert(len(ownership) == self.board.get_number_of_areas())

        for area_name, player_name in ownership.items():
            area = self.board.get_area_by_name(area_name)
            self.assign_area(area, self.players[player_name])

    def adjust_player_order(self, nicknames_order):
        renumbering = {old_name: nicknames_order.index(player.nickname)+1 for old_name, player in self.players.items()}

        self.players = {renumbering[old_name]: player for old_name, player in self.players.items()}
        for name, player in self.players.items():
            player.name = name

        self.client_sockets = {renumbering[old_name]: socket for old_name, socket in self.client_sockets.items()}

        registered_nicknames_rev = {player.nickname: player_name for player_name, player in self.players.items()}
        assert(len(nicknames_order) == len(registered_nicknames_rev))
        assert(set(nicknames_order) == set(registered_nicknames_rev.keys()))

        self.players_order = []
        for nick in nicknames_order:
            self.players_order.append(registered_nicknames_rev[nick])

        self.set_first_player()

    def report_player_order(self):
        self.logger.info('Player order: {}'.format([(name, self.players[name].nickname) for name in self.players_order]))


class UnlimitedDeployment:
    def __init__(self, max_val):
        self.max_dice_per_area = max_val

    def __call__(self, player):
        return len(player.get_areas()) * self.max_dice_per_area


class LimitedDeployment:
    def __init__(self, max_val):
        xs = np.arange(1, 41)
        incs = max_val * np.ones(len(xs), dtype=np.int)

        for i in range(1, 5):
            incs -= np.heaviside(xs - i*7 - 0.5, 1).astype(np.int)

        self.vals = np.cumsum(incs)

    def __call__(self, player):
        nb_areas = len(player.get_areas())
        return int(self.vals[nb_areas-1])
