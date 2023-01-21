#!/usr/bin/env python3

import argparse
import numpy as np
import pickle


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--output', required=True, help='where to put all the games')
    parser.add_argument('games', nargs='*', help='where the games are stored')
    args = parser.parse_args()

    all_games = []

    for fn in args.games:
        with open(fn, 'rb') as f:
            all_games.extend(pickle.load(f))

    with open(args.output, 'wb') as f:
        pickle.dump(all_games, f)

if __name__ == '__main__':
    main()
