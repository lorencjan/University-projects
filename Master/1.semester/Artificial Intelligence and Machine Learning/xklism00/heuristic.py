# File: xklism00.py
# Solution: SUI - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Michal Kliš, Michal Sova, Jana Gregorová
# Faculty: Faculty of information technology VUT
# Description: Class responsible for the ML model-94 and game state evaluation.


import torch
from dicewars.ai.xklism00.neural_net import NeuralNet
from dicewars.ai.xklism00.utils import Utils
from dicewars.ai.utils import probability_of_holding_area


class Heuristic:

    def __init__(self, model_location):
        pass
        self.model = NeuralNet()
        self.model.load_state_dict(torch.load(model_location))
        self.model.eval()

    def evaluate_board_state(self, board, player_name):
        """ Computes players chance of winning for current game state using the trained NN model.
            Parameters
            ----------
            board :
                Instance of a board which is evaluated.
            player_name :
                Name of the player for whom to get the score.
            Returns
            -------
            float
                Score == probability of winning for the specified player.
        """

        scores = self.evaluate(board)
        return scores[player_name]

    def evaluate_transfer_score(self, board, player_name, area):
        """ Evaluates score for an area base on the model and it's holding probability.
            Parameters
            ----------
            board :
                Instance of a board which is evaluated.
            player_name :
                Name of the player making transfer.
            area :
                Area which is being evaluated.
            Returns
            -------
            float
                Normalized sum holding and heuristic probabilities.
        """

        hold_prob = probability_of_holding_area(board, area.get_name(), area.get_dice(), player_name)
        heuristic_probs = self.evaluate(board)
        return (hold_prob + heuristic_probs[player_name]) / 2

    def evaluate(self, board):
        """ Gets the model results for current board state for all players.
            Parameters
            ----------
            board :
                Instance of a board which is evaluated.
            Returns
            -------
            dict
                Probability of winning for each player.
        """

        max_region_sizes = [len(Utils.get_largest_region(board, p)) for p in range(1, 5)]
        vector = Utils.vectorize_game_state(board, max_region_sizes)
        probs = self.model(torch.Tensor(vector).reshape(1, -1))
        return {i: prob.item() for i, prob in enumerate(probs.flatten(), 1)}
