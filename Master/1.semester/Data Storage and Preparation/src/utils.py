# File: utils.py
# Solution: UPA - project
# Date: 7.11.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Contains useful helper functions.


from os import makedirs
from os.path import exists, join


def write_to_file(directory, filename, content):
    """ Helper method for easier writing to files. """

    create_dir(directory)
    with open(join(directory, filename), "w") as f:
        f.write(content)


def save_fig(plt, name, directory="img", tight_layout=True):
    """ Helper method for saving plots. """

    create_dir(directory)
    path = join(directory, name)

    for ax in plt.gcf().axes:
        ax.set_xlabel(ax.get_xlabel(), fontsize=16)
        ax.set_ylabel(ax.get_ylabel(), fontsize=16)
        ax.tick_params(axis="both", labelsize=14)

    if tight_layout:
        plt.gcf().tight_layout() 

    plt.savefig(path)


def create_dir(directory):
    """ Creates directory if it doesn't exist. """

    if not exists(directory):
        makedirs(directory)

    return directory
