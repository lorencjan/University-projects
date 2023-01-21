import numpy
from dicewars.client.game.board import Board
from dicewars.client.game.area import Area
from typing import Iterator, Tuple
import pickle


def sigmoid(a):
    """Logistic sigmoid

    Parameters
    ----------
    a : numpy.int64
        Product of input vector and trained weights

    Returns
    -------
    numpy.float64
        Result of the sigmoid from the interval [0, 1]
    """
    return 1 / (1 + numpy.exp(-a))


def probability_of_holding_area(board, area_name, area_dice, player_name):
    """Estimate probability of holding an area until next turn

    Parameters
    ----------
    board : Board
    area_name : int
    area_dice : int
    player_name : int
        Owner of the area

    Returns
    -------
    float
        Estimated probability
    """
    area = board.get_area(area_name)
    probability = 1.0
    for adj in area.get_adjacent_areas_names():
        adjacent_area = board.get_area(adj)
        if adjacent_area.get_owner_name() != player_name:
            enemy_dice = adjacent_area.get_dice()
            if enemy_dice == 1:
                continue
            lose_prob = attack_succcess_probability(enemy_dice, area_dice)
            hold_prob = 1.0 - lose_prob
            probability *= hold_prob
    return probability


def probability_of_successful_attack(board, atk_area, target_area):
    """Calculate probability of attack success

    Parameters
    ----------
    board : Board
    atk_area : int
    target_area : int

    Returns
    -------
    float
        Calculated probability
    """
    atk = board.get_area(atk_area)
    target = board.get_area(target_area)
    atk_power = atk.get_dice()
    def_power = target.get_dice()
    return attack_succcess_probability(atk_power, def_power)


def attack_succcess_probability(atk, df):
    """Dictionary with pre-calculated probabilities for each combination of dice

    Parameters
    ----------
    atk : int
        Number of dice the attacker has
    df : int
        Number of dice the defender has

    Returns
    -------
    float
    """
    return {
        2: {
            1: 0.83796296,
            2: 0.44367284,
            3: 0.15200617,
            4: 0.03587963,
            5: 0.00610497,
            6: 0.00076625,
            7: 0.00007095,
            8: 0.00000473,
        },
        3: {
            1: 0.97299383,
            2: 0.77854938,
            3: 0.45357510,
            4: 0.19170096,
            5: 0.06071269,
            6: 0.01487860,
            7: 0.00288998,
            8: 0.00045192,
        },
        4: {
            1: 0.99729938,
            2: 0.93923611,
            3: 0.74283050,
            4: 0.45952825,
            5: 0.22044235,
            6: 0.08342284,
            7: 0.02544975,
            8: 0.00637948,
        },
        5: {
            1: 0.99984997,
            2: 0.98794010,
            3: 0.90934714,
            4: 0.71807842,
            5: 0.46365360,
            6: 0.24244910,
            7: 0.10362599,
            8: 0.03674187,
        },
        6: {
            1: 0.99999643,
            2: 0.99821685,
            3: 0.97529981,
            4: 0.88395347,
            5: 0.69961639,
            6: 0.46673060,
            7: 0.25998382,
            8: 0.12150697,
        },
        7: {
            1: 1.00000000,
            2: 0.99980134,
            3: 0.99466336,
            4: 0.96153588,
            5: 0.86237652,
            6: 0.68516499,
            7: 0.46913917,
            8: 0.27437553,
        },
        8: {
            1: 1.00000000,
            2: 0.99998345,
            3: 0.99906917,
            4: 0.98953404,
            5: 0.94773146,
            6: 0.84387382,
            7: 0.67345564,
            8: 0.47109073,
        },
    }[atk][df]


def possible_attacks(board: Board, player_name: int) -> Iterator[Tuple[Area, Area]]:
    for area in board.get_player_border(player_name):
        if not area.can_attack():
            continue

        neighbours = area.get_adjacent_areas_names()

        for adj in neighbours:
            adjacent_area = board.get_area(adj)
            if adjacent_area.get_owner_name() != player_name:
                yield (area, adjacent_area)


def save_state(f, board, player_name, players_order):
    save_game = {
        'player_name': player_name,
        'board': board,
        'current_player_name': player_name,
        'order': players_order,
    }

    pickle.dump(save_game, f)
