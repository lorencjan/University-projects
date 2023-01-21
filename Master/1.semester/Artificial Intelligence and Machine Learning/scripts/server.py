#!/usr/bin/env python3

from argparse import ArgumentParser
import configparser
import logging
import random

from itertools import cycle

from dicewars.server.board import Board
from dicewars.server.generator import BoardGenerator
from dicewars.server.game import Game


from utils import get_logging_level


def area_player_mapping(nb_players, nb_areas):
    assignment = {}
    unassigned_areas = list(range(1, nb_areas+1))
    player_cycle = cycle(range(1, nb_players+1))

    while unassigned_areas:
        player_no = next(player_cycle)
        area_no = random.choice(unassigned_areas)
        assignment[area_no] = player_no
        unassigned_areas.remove(area_no)

    return assignment


def continuous_area_player_mapping(nb_players, board):
    assignment = {}
    nb_areas = board.get_number_of_areas()
    unassigned_areas = set(range(1, nb_areas+1))
    player_cycle = cycle(range(1, nb_players+1))

    def unassigned_neighbours(area):
        return {area for area in board.get_area_by_name(area_no).get_adjacent_areas_names() if area in unassigned_areas}

    def assign_area(area_no, player):
        assignment[area_no] = player_no
        unassigned_areas.remove(area_no)

    available_to_player = dict()
    for player_no in range(1, nb_players+1):
        area_no = random.choice(list(unassigned_areas))
        assign_area(area_no, player_no)
        available_to_player[player_no] = unassigned_neighbours(area_no)

    while unassigned_areas:
        player_no = next(player_cycle)
        available_to_player[player_no] &= unassigned_areas

        if not available_to_player[player_no]:
            logging.info(f"Player {player_no} has no options more")
            continue

        area_no = random.choice(list(available_to_player[player_no]))
        assign_area(area_no, player_no)
        available_to_player[player_no].remove(area_no)

        available_to_player[player_no] |= unassigned_neighbours(area_no)

    return assignment


def players_areas(ownership, the_player):
    return [area for area, player in ownership.items() if player == the_player]


def assign_dice_flat(board, nb_players, ownership, dice_density):
    for area in board.areas.values():
        area.set_dice(dice_density)


def assign_dice_random(board, nb_players, ownership, dice_density, max_dice_per_area=8):
    dice_total = dice_density * board.get_number_of_areas()

    for player in range(1, nb_players+1):
        player_dice = dice_total // nb_players

        available_areas = [board.get_area_by_name(area_name) for area_name in players_areas(ownership, player)]

        # each area has to have at least one die
        for area in available_areas:
            area.set_dice(1)
            player_dice -= 1

        while player_dice >= 0 and available_areas:
            area = random.choice(available_areas)
            if area.get_dice() >= max_dice_per_area:
                available_areas.remove(area)
            else:
                area.dice += 1
                player_dice -= 1


def create_board(board_config):
    generator = BoardGenerator()
    return Board(generator.generate_board(board_config.getint('BoardSize')))


def produce_area_assignment(board_config, board, nb_players):
    area_assignment_method = board_config.get('AreaAssignment')
    if area_assignment_method == 'orig':
        area_ownership = area_player_mapping(nb_players, board.get_number_of_areas())
    elif area_assignment_method == 'continuous':
        area_ownership = continuous_area_player_mapping(nb_players, board)
    else:
        raise ValueError(f'Unsupported area assignment method "{area_assignment_method}"')

    return area_ownership


def assign_dice(board_config, board, nb_players, area_ownership):
    dice_assignment_method = board_config.get('DiceAssignment')
    dice_density = board_config.getint('DiceDensity')
    if dice_assignment_method == 'orig':
        assign_dice_random(
            board=board,
            nb_players=nb_players,
            ownership=area_ownership,
            dice_density=dice_density,
        )
    elif dice_assignment_method == 'flat':
        assign_dice_flat(board, nb_players, area_ownership, dice_density)
    else:
        raise ValueError(f'Unsupport dice assignment method "{dice_assignment_method}"')


def main():
    """
    Server for Dice Wars
    """

    parser = ArgumentParser(prog='Dice_Wars-server')
    parser.add_argument('-n', '--number-of-players', help="Number of players", type=int, default=2)
    parser.add_argument('-p', '--port', help="Server port", type=int, default=5005)
    parser.add_argument('-a', '--address', help="Server address", default='127.0.0.1')
    parser.add_argument('-d', '--debug', help="Enable debug output", default='WARN')
    parser.add_argument('-b', '--board', help="Random seed to be used for board creating", type=int)
    parser.add_argument('-o', '--ownership', help="Random seed to be used for province assignment", type=int)
    parser.add_argument('-s', '--strength', help="Random seed to be used for dice assignment", type=int)
    parser.add_argument('-f', '--fixed', help="Random seed to be used for player order and dice rolls", type=int)
    parser.add_argument('-r', '--order', nargs='+',
                        help="Random seed to be used for dice assignment")
    args = parser.parse_args()

    config = configparser.ConfigParser()
    config.read('dicewars.config')
    board_config = config['BOARD']
    game_config = config['GAME']

    log_level = get_logging_level(args)

    logging.basicConfig(level=log_level)
    logger = logging.getLogger('SERVER')
    logger.debug("Command line arguments: {0}".format(args))

    random.seed(args.board)
    board = create_board(board_config)

    random.seed(args.ownership)
    area_ownership = produce_area_assignment(board_config, board, args.number_of_players)

    random.seed(args.strength)
    assign_dice(board_config, board, args.number_of_players, area_ownership)

    random.seed(args.fixed)
    game = Game(board, area_ownership, args.number_of_players, game_config, args.address, args.port, args.order)
    game.run()


if __name__ == '__main__':
    main()
