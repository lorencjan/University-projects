# File: data_collector.py
# Solution: SUI - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Michal Kliš, Michal Sova, Jana Gregorová
# Faculty: Faculty of information technology VUT
# Description: Class collecting game data_test for the training.
#              BELONGS TO THE SERVER DIRECTORY


import importlib
import pickle
import pandas as pd
from os import makedirs, listdir
from os.path import join, exists


class DataCollector:

    def __init__(self):
        self.game_states = []

    def save_state(self, board, players):
        """ Saves current board state.
            ----------
            board :
                Instance of a board which state to save.
            players :
                Instance of a board which state to save.
        """

        max_region_sizes = [p.get_largest_region(board) for p in sorted(players.values(), key=lambda x: x.name)]
        self.game_states.append(self.get_xklism00_utils().vectorize_game_state(board, max_region_sizes))

    def save_game_result(self, board, players):
        """ Saves the game result data_test for further training.
            ----------
            board :
                Instance of a board on which the game was playes. All areas should belong to 1 player.
            players :
                Players that participated in the game.
        """

        data_dir = join("supp-xklism00", "raw_data")
        if not exists(data_dir):
            makedirs(data_dir)

        num_of_areas = board.get_number_of_areas()
        for _, player in players.items():
            if player.get_number_of_areas() == num_of_areas:
                df = pd.DataFrame({"BoardState": self.game_states, "Winner": int(player.get_name())})
                file_name = join(data_dir, str(len(listdir(data_dir)) + 1))
                with open(file_name, 'wb') as file:
                    pickle.dump(df, file)
                break

    def get_xklism00_utils(self):
        """ We need to access Utils namespace of the AI module to use. This gets us the module. """

        utils = importlib.import_module("dicewars.ai.xklism00")
        return utils.Utils
