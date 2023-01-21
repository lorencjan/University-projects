import json
import logging
import socket
from queue import Queue
from time import sleep

from .board import Board
from .player import Player
from dicewars.client.socket_listener import SocketListener


class Game:
    """Represantation of the game state
    """
    def __init__(self, addr, port, hello_msg):
        """
        Parameters
        ----------
        addr : str
            Server address
        port : int
            Server port
        """
        self.logger = logging.getLogger('CLIENT')

        self.buffer = 65535
        self.battle_in_progress = False

        self.server_address = addr
        self.server_port = port
        self.players = {}

        i = 0
        while True:
            try:
                self.init_socket()
                break
            except ConnectionRefusedError as e:
                if i > 100:
                    self.logger.error("Connection to server refused: {0}".format(e))
                    exit(1)
                i += 1
                sleep(0.01)

        try:
            self.socket.send(str.encode(json.dumps(hello_msg)))
        except BrokenPipeError:
            self.logger.error("Connection to server broken.")
            exit(1)

        self.start_socket_daemon()
        while self.input_queue.empty():
            pass
        msg = self.input_queue.get()

        self.logger.debug("Received message: {0}\n".format(msg))  # TODO
        if msg['type'] == 'game_start':
            self.player_name = msg['player']
            self.add_players(int(msg['no_players']), msg['score'])
            self.board = Board(msg['areas'], msg['board'])
            self.current_player = self.players[msg['current_player']]
            self.current_player_name = msg['current_player']
            self.players_order = msg['order']
        else:
            self.logger.error("Did not receive game state from server.")
            exit(1)

        self.logger.info("This is player name {}, the players order is {}".format(self.player_name, self.players_order))

    ##################
    # INITIALIZATION #
    ##################
    def add_players(self, number_of_players, score):
        """Create Players instances

        Parameters
        ----------
        number_of_players : int
        score : list of int
            Initial scores of all players
        """
        self.number_of_players = number_of_players

        for i in range(1, number_of_players + 1):
            self.players[i] = Player(i, score[str(i)])

    ##############
    # NETWORKING #
    ##############
    def send_message(self, type, attacker=None, defender=None):
        """Send message to the server

        Parameters
        ----------
        type : str
        attacker : int
            Name of attacking area
        defender : int
            Name of defending area
        """
        if type == 'close':
            msg = {'type': 'close'}
        elif type == 'battle':
            msg = {
                'type': 'battle',
                'atk': attacker,
                'def': defender
            }
        elif type == 'transfer':
            msg = {
                'type': 'transfer',
                'src': attacker,
                'dst': defender
            }
        elif type == 'end_turn':
            msg = {'type': 'end_turn'}
            self.logger.debug("Sending end_turn message.")

        try:
            self.socket.send(str.encode(json.dumps(msg)))
        except BrokenPipeError:
            self.logger.error("Connection to server broken.")
            exit(1)

    def init_socket(self):
        """Socket initialization
        """
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.connect((self.server_address, self.server_port))

    def start_socket_daemon(self):
        """Start message collecting daemon
        """
        self.input_queue = Queue()
        self.socket_listener = SocketListener(self.socket, self.buffer, self.input_queue)
        self.socket_listener.daemon = True
        self.socket_listener.start()
        self.logger.debug("Started socket daemon.")

    def process_battle_msg(self, msg):
        assert msg['type'] == 'battle'

        atk_data = msg['result']['atk']
        def_data = msg['result']['def']
        attacker = self.board.get_area(str(atk_data['name']))
        attacker.set_dice(atk_data['dice'])
        atk_name = attacker.get_owner_name()

        defender = self.board.get_area(def_data['name'])
        defender.set_dice(def_data['dice'])
        def_name = defender.get_owner_name()

        if def_data['owner'] == atk_data['owner']:
            defender.set_owner(atk_data['owner'])
            self.players[atk_name].set_score(msg['score'][str(atk_name)])
            self.players[def_name].set_score(msg['score'][str(def_name)])

    def process_transfer_msg(self, msg):
        assert msg['type'] == 'transfer'

        src_data = msg['result']['src']
        source = self.board.get_area(str(src_data['name']))
        source.set_dice(src_data['dice'])

        dst_data = msg['result']['dst']
        destination = self.board.get_area(str(dst_data['name']))
        destination.set_dice(dst_data['dice'])

    def process_end_turn_msg(self, msg):
        assert msg['type'] == 'end_turn'

        current_player = self.players[self.current_player_name]

        for area in msg['areas']:
            owner_name = msg['areas'][area]['owner']

            area_object = self.board.get_area(int(area))

            area_object.set_owner(owner_name)
            area_object.set_dice(msg['areas'][area]['dice'])

        current_player.deactivate()
        self.current_player_name = msg['current_player']
        self.current_player = self.players[msg['current_player']]
        self.players[self.current_player_name].activate()

        for i, player in self.players.items():
            player.set_reserve(msg['reserves'][str(i)])
