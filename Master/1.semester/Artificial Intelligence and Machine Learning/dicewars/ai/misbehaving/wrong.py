import random
import logging

from dicewars.client.ai_driver import BattleCommand, EndTurnCommand


class AI:
    """Naive player agent

    This agent performs all possible moves in random order
    """

    def __init__(self, player_name, board, players_order, max_transfers):
        """
        Parameters
        ----------
        game : Game
        """
        self.player_name = player_name
        self.logger = logging.getLogger('AI')

    def ai_turn(self, board, nb_moves_this_turn, nb_transfers_this_turn, nb_turns_this_game, time_left):
        """AI agent's turn

        Get a random area. If it has a possible move, the agent will do it.
        If there are no more moves, the agent ends its turn.
        """
        if nb_moves_this_turn == 2:
            self.logger.debug("I'm too well behaved. Let others play now.")
            return EndTurnCommand()

        source = random.choice(list(range(25)))
        target = random.choice(list(range(25)))
        return BattleCommand(source, target)
