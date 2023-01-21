import argparse
from Runner import Runner


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("-t", "--task", default="houses",
                        help="valid options: 'houses' | 'houses-kfold' | 'sinus' | 'linear'")
    args = parser.parse_args()

    if args.task == "houses":
        Runner.houses(False)
    elif args.task == "houses-kfold":
        Runner.houses_k_fold(10)
    elif args.task == "sinus":
        Runner.approximate_sine_function(False)
    elif args.task == "linear":
        Runner.linear_regression(False)
    else:
        print("Invalid task specification!")
