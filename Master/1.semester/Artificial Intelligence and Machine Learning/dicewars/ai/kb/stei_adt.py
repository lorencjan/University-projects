import logging

from dicewars.client.ai_driver import BattleCommand, EndTurnCommand, TransferCommand
from dicewars.ai.kb.move_selection import get_transfer_from_endangered, get_transfer_to_border
from dicewars.ai.dt import stei


class AI:
    """Agent combining STEi with both aggressive and defensive transfers

    First, it tries to transfer forces towards borders (blindly, 2/3 of transfers available),
    then it performs attacks following the dt.stei AI and finally, it pull dice from danger
    (locally optimal, 1/3 of transfers).
    """
    def __init__(self, player_name, board, players_order, max_transfers):
        self.player_name = player_name
        self.logger = logging.getLogger('AI')
        self.max_transfers = max_transfers

        self.stei = stei.AI(player_name, board, players_order, max_transfers)
        self.reserved_evacs = max_transfers // 3
        self.logger.debug(f'STEi_adt will keep {self.reserved_evacs} transfers for evacuations')

        self.stage = 'attack'

    def ai_turn(self, board, nb_moves_this_turn, nb_transfers_this_turn, nb_turns_this_game, time_left):
        if nb_transfers_this_turn + self.reserved_evacs < self.max_transfers:
            transfer = get_transfer_to_border(board, self.player_name)
            if transfer:
                return TransferCommand(transfer[0], transfer[1])
        else:
            self.logger.debug(f'Already did {nb_transfers_this_turn}/{self.max_transfers} transfers, reserving {self.reserved_evacs} for evac, skipping further aggresive ones')

        if self.stage == 'attack':
            stei_move = self.stei.ai_turn(board, nb_moves_this_turn, nb_transfers_this_turn, nb_turns_this_game, time_left)
            if isinstance(stei_move, BattleCommand):
                return stei_move
            else:
                self.stage = 'evac'

        if self.stage == 'evac':
            if nb_transfers_this_turn < self.max_transfers:
                transfer = get_transfer_from_endangered(board, self.player_name)
                if transfer:
                    return TransferCommand(transfer[0], transfer[1])
            else:
                self.logger.debug(f'Already did {nb_transfers_this_turn}/{self.max_transfers} transfers, skipping further')

        self.stage = 'attack'
        return EndTurnCommand()
