# File: xklism00.py
# Solution: SUI - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Michal Kliš, Michal Sova, Jana Gregorová
# Faculty: Faculty of information technology VUT
# Description: AI agent implementation.

import logging
import copy

from dicewars.ai.xklism00.utils import Utils
from dicewars.ai.xklism00.heuristic import Heuristic
from dicewars.client.ai_driver import BattleCommand, EndTurnCommand, TransferCommand


class AI:
    def __init__(self, player_name, board, players_order, max_transfers):
        self.player_name = player_name
        self.logger = logging.getLogger('AI')
        self.players_order = players_order
        self.max_transfers = max_transfers
        self.attack_mode = True
        self.heuristic = Heuristic("dicewars/ai/xklism00/model")

    def ai_turn(self, board, nb_moves_this_turn, nb_transfers_this_turn, nb_turns_this_game, time_left):
        """ Repetitive AI turn. """

        if self.attack_mode:
            # start turn with strengthening our borders
            if nb_transfers_this_turn < self.max_transfers - 1:
                transfer = Utils.transfer_to_border(board, self.player_name, self.heuristic)
                if transfer:
                    return TransferCommand(*transfer)

            # perform attacks base on expectiminimax
            depth = 3 if time_left >= 20 else 2 if time_left >= 10 else 1
            best_move = self.run_expectiminimax(board, self.player_name, depth)
            if best_move:
                return BattleCommand(best_move["src"].get_name(), best_move["dst"].get_name())

        # we kept some transfers for the end of turn to save dice from undefendable areas
        self.attack_mode = False
        if nb_transfers_this_turn < self.max_transfers:
            transfer = Utils.transfer_from_border(board, self.player_name, self.heuristic)
            if transfer:
                return TransferCommand(*transfer)

        self.attack_mode = True
        return EndTurnCommand()

    def run_expectiminimax(self, board, player_name, depth=1):
        """ Runs expectiminimax algorithm by simulating other players moves.
            It doesn't take into account unreasonable attacks such as attacking with fewer dice etc.
            Parameters
            ----------
            board :
                Instance of a board which the current player uses.
                It gets copied for each simulation so that the parent is not influenced.
            player_name :
                Name of the current player.
            depth :
                Depth into which the algorithm should search.
            Returns
            -------
            obj {src, dst, score}
                Object representing best move with its score.
        """

        reasonable_attacks = []
        for src, dst, prob_score in Utils.get_possible_attacks(board, player_name):
            sim_board = copy.deepcopy(board)                             # not to mess up the board for the simulation
            Utils.claim_area(sim_board, src.get_name(), dst.get_name())  # simulate attack

            if depth == 0:  # no further expansions (simulations of moves)
                score = self.heuristic.evaluate_board_state(sim_board, player_name) * prob_score
                reasonable_attacks.append({"src": src, "dst": dst, "score": score})
                continue
            
            # we dive deeper to another level
            other_players = [p for p in Utils.order_players(self.players_order, player_name) if p != player_name]
            alive_players = [p for p in other_players if len(Utils.get_largest_region(sim_board, p)) > 0]
            for player in alive_players:
                move = self.run_expectiminimax(sim_board, player, depth - 1)
                if move:
                    Utils.claim_area(sim_board, move["src"].get_name(), move["dst"].get_name())

            score = self.heuristic.evaluate_board_state(sim_board, player_name) * prob_score
            reasonable_attacks.append({"src": src, "dst": dst, "score": score})
        
        # return move with the highest score
        return sorted(reasonable_attacks, key=lambda x: x["score"])[-1] if len(reasonable_attacks) > 0 else None
