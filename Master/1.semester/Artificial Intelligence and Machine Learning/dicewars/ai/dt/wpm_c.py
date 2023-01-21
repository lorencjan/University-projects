import numpy
import logging

from ..utils import probability_of_successful_attack, sigmoid
from ..utils import possible_attacks

from dicewars.client.ai_driver import BattleCommand, EndTurnCommand


class AI:
    """Agent using Win Probability Maximization (WPM) using logarithms
    of player scores and dice

    This agent estimates win probability given the current state of the game.
    As a feature to describe the state, a vector of logarithms of players' dice
    and scores is used. The agent choses such moves, that will have the highest
    improvement in the estimated probability.
    """
    def __init__(self, player_name, board, players_order, max_transfers):
        """
        Parameters
        ----------
        game : Game

        Attributes
        ----------
        players_order : list of int
            Names of players in the order they are playing, with the agent being first
        weights : dict of numpy.array
            Weights for estimating win probability
        largest_region: list of int
            Names of areas in the largest region
        """
        self.player_name = player_name
        self.logger = logging.getLogger('AI')
        self.players = board.nb_players_alive()
        self.largest_region = []

        self.players_order = players_order
        while self.player_name != self.players_order[0]:
            self.players_order.append(self.players_order.pop(0))

        self.weights = {
            2: numpy.array([1.30214778, 2.25563871, -1.30214778, -2.25563871]),
            3: numpy.array([1.03427841, 0.50262886, -0.78619448, -0.31264667,
                            -0.74070513, -0.3344083]),
            4: numpy.array([1.04279419, 0.25416893, -0.64830571, -0.15321224,
                            -0.64217824, -0.11354054, -0.59113493, -0.19902261]),
            5: numpy.array([0.88792394, 0.23898045, -0.50630318, -0.10684734,
                            -0.48406202, -0.12877724, -0.48004353, -0.17429738,
                            -0.51195613, -0.12572176]),
            6: numpy.array([0.84452717, 0.20915755, -0.4275969, -0.12319906,
                            -0.438397, -0.11476484, -0.44610219, -0.10640943,
                            -0.42926595, -0.15994294, -0.40215393, -0.12508173]),
            7: numpy.array([0.77043331, 0.22744643, -0.34448306, -0.16104125,
                            -0.34304867, -0.16545059, -0.36316993, -0.14238659,
                            -0.37359036, -0.13535348, -0.34917492, -0.13725688,
                            -0.36908313, -0.11803061]),
            8: numpy.array([0.71518557, 0.2580538, -0.3303392, -0.13374949,
                            -0.3288953, -0.16076534, -0.31261043, -0.14316612,
                            -0.31785557, -0.16003507, -0.31410674, -0.16487769,
                            -0.33290964, -0.12624279, -0.33843017, -0.14888412]),
        }[self.players]
        numpy.warnings.filterwarnings('ignore')

    def ai_turn(self, board, nb_moves_this_turn, nb_transfers_this_turn, nb_turns_this_game, time_left):
        """AI agent's turn

        This agent estimates probability to win the game from the feature vector associated
        with the outcome of the move and chooses such that has highest improvement in the
        probability.
        """
        self.board = board
        self.logger.debug("Looking for possible turns.")
        turns = self.possible_turns()
        if turns and turns[0][0] != 'end':
            turn = turns[0]
            area_name = turn[0]
            self.logger.debug("Possible turn: {}".format(turn))
            atk_area = self.board.get_area(turn[0])
            atk_power = atk_area.get_dice()

            if turn[2] >= -0.05 or atk_power == 8:
                return BattleCommand(turn[0], turn[1])

        if turns and turns[0][0] == 'end':
            for i in range(1, len(turns)):
                area_name = turns[i][0]
                atk_area = self.board.get_area(area_name)
                atk_power = atk_area.get_dice()
                if atk_power == 8:
                    return BattleCommand(area_name, turns[i][1])

        self.logger.debug("Don't want to attack anymore.")
        return EndTurnCommand()

    def get_features(self, end_turn=False):
        """Get features associated with a move

        Parameters
        ----------
        end_turn : bool
            The move is ending the turn

        Returns
        -------
        list of int
        """
        features = []
        for p in self.players_order:
            score = numpy.log(self.get_score_by_player(p) + 1)
            if end_turn and p == self.player_name:
                dice = numpy.log(self.board.get_player_dice(p) + self.get_score_by_player(p) + 1)
            else:
                dice = numpy.log(self.board.get_player_dice(p) + 1)
            features.append(score)
            features.append(dice)
        return features

    def possible_turns(self):
        """Get list of possible turns with the associated improvement
        in estimated win probability. The list is sorted in descending order
        with respect to the improvement.
        """
        turns = []
        name = self.player_name

        features = self.get_features()
        wp_start = numpy.log(sigmoid(numpy.dot(numpy.array(features), self.weights)))

        end_features = self.get_features(end_turn=True)
        wp_end = numpy.log(sigmoid(numpy.dot(numpy.array(end_features), self.weights)))
        improvement = wp_end - wp_start

        turns.append(['end', 0, improvement])

        for source, target in possible_attacks(self.board, self.player_name):
            area_name = source.get_name()
            atk_power = source.get_dice()
            def_power = target.get_dice()
            opponent_name = target.get_owner_name()
            # check whether the attack would expand the largest region
            increase_score = False
            if area_name in self.largest_region:
                increase_score = True
            else:
                for n in target.get_adjacent_areas_names():
                    if n in self.largest_region:
                        increase_score = True
                        break

            a_dice = self.board.get_player_dice(name)
            a_score = self.get_score_by_player(name)
            if increase_score:
                a_score += 1

            atk_dice = {
                "current": a_dice,
                "win": a_dice + a_score,
                "loss": a_dice + a_score - atk_power + 1,
            }

            d_dice = self.board.get_player_dice(opponent_name)
            def_dice = {
                "loss": d_dice,
                "win": d_dice - def_power,
            }

            atk_prob = probability_of_successful_attack(self.board, area_name, target.get_name())
            opponent_idx = self.players_order.index(opponent_name) * 2 + 1
            win_features = [d for d in features]
            win_features[1] = numpy.log(atk_dice["win"] + 1)
            win_features[opponent_idx] = numpy.log(def_dice["win"] + 1)

            loss_features = [d for d in features]
            loss_features[1] = numpy.log(atk_dice["loss"] + 1)
            loss_features[opponent_idx] = numpy.log(def_dice["loss"] + 1)

            wp_win = sigmoid(numpy.dot(numpy.array(win_features), self.weights))
            wp_loss = sigmoid(numpy.dot(numpy.array(loss_features), self.weights))

            wp_win = sigmoid(numpy.dot(numpy.array(win_features), self.weights))
            wp_loss = sigmoid(numpy.dot(numpy.array(loss_features), self.weights))
            total_prob = (wp_win * atk_prob) + (wp_loss * (1.0 - atk_prob))
            wp_atk = numpy.log(total_prob)

            improvement = wp_atk - wp_start
            turns.append([area_name, target.get_name(), improvement])

        return sorted(turns, key=lambda turn: turn[2], reverse=True)

    def get_score_by_player(self, player_name, skip_area=None):
        """Get score of a player

        Parameters
        ----------
        player_name : int
        skip_area : int
            Name of an area to be excluded from the calculation

        Returns
        -------
        int
            score of the player
        """
        players_regions = self.board.get_players_regions(self.player_name, skip_area=skip_area)
        max_region_size = max(len(region) for region in players_regions)

        return max_region_size

    def get_largest_region(self):
        """Get size of the largest region, including the areas within

        Attributes
        ----------
        largest_region : list of int
            Names of areas in the largest region

        Returns
        -------
        int
            Number of areas in the largest region
        """
        self.largest_region = []

        players_regions = self.board.get_players_regions(self.player_name)
        max_region_size = max(len(region) for region in players_regions)
        max_sized_regions = [region for region in players_regions if len(region) == max_region_size]

        for region in max_sized_regions:
            for area in region:
                self.largest_region.append(area)
        return max_region_size
