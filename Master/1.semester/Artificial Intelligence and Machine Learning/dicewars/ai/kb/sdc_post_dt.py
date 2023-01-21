import logging

from dicewars.client.ai_driver import BattleCommand, EndTurnCommand, TransferCommand
from dicewars.ai.kb.move_selection import get_sdc_attack, get_transfer_from_endangered


class AI:
    """Agent using Strength Difference Checking (SDC) strategy

    This agent prefers moves with highest strength difference
    and doesn't make moves against areas with higher strength.
    """
    def __init__(self, player_name, board, players_order, max_transfers):
        """
        Parameters
        ----------
        game : Game

        Attributes
        ----------
            Areas that can make an attack
        """
        self.player_name = player_name
        self.logger = logging.getLogger('AI')
        self.max_transfers = max_transfers

        self.stage = 'attack'

    def ai_turn(self, board, nb_moves_this_turn, nb_transfers_this_turn, nb_turns_this_game, time_left):
        """AI agent's turn

        Creates a list with all possible moves along with associated strength
        difference. The list is then sorted in descending order with respect to
        the SD. A move with the highest SD is then made unless the highest
        SD is lower than zero - in this case, the agent ends its turn.
        """

        if self.stage == 'attack':
            attack = get_sdc_attack(board, self.player_name)
            if attack:
                return BattleCommand(attack[0], attack[1])
            else:
                self.stage = 'transfer'

        if self.stage == 'transfer':
            if nb_transfers_this_turn < self.max_transfers:
                transfer = get_transfer_from_endangered(board, self.player_name)
                if transfer:
                    return TransferCommand(transfer[0], transfer[1])
            else:
                self.logger.debug(f'Already did {nb_transfers_this_turn}/{self.max_transfers} transfers, skipping further')

        self.stage = 'attack'
        return EndTurnCommand()
