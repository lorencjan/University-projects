# File: moire_creator.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Class capable of creating moiré effects.

import latticegen
import numpy as np


class MoireCreator:
    """ Class for creating moiré lattices. """

    def __init__(self, size):
        self.size = size

    def create_shift(self):
        """ Creates shift in the lattice. Taken from latticegen docs. No need to parameterize or randomize
            as it has different effect on various lattices.

            Returns
            -------
            _ : np.array
                Shift that can be applied to a lattice
        """

        size = self.size / 2
        xp, yp = np.mgrid[-size:size, -size:size]
        x_shift = 0.5 * xp * np.exp(-0.5 * ((xp / (2 * size / 8)) ** 2 + 1.2 * (yp / (2 * size / 6)) ** 2))
        return np.stack([x_shift, np.zeros_like(x_shift)])

    def create_random_lattice(self, max_lattice_order=3, special_moire_symmetry_prob=0.5):
        """ Creates a random moiré lattice base on size, rotation, order and symmetry.

            Parameters
            ----------
            max_lattice_order : int, optional
                Defines how much the lattice is resolved as single dots.
            special_moire_symmetry_prob : float, optional
                Most common moiré effect are lines (symmetry=1).
                This parameter specifies probability of more intricate patterns.

            Returns
            -------
            _ : np.array
                Moiré lattice
        """

        r_k = np.random.uniform(0.01, 0.25)  # how big or seemingly zoomed in it is
        theta = np.random.uniform(0, 90)     # rotation
        order = np.random.randint(1, max_lattice_order + 1)
        symmetry = np.random.randint(2, 6) if np.random.random() < special_moire_symmetry_prob else 1
        return latticegen.anylattice_gen(r_k=r_k, theta=theta, order=order, symmetry=symmetry,
                                         size=self.size, shift=self.create_shift())

    def combine_lattice(self, num_of_lattices=3, max_lattice_order=3, special_moire_symmetry_prob=0.5):
        """ Creates a random moiré lattice base on size, rotation, order and symmetry.

            Parameters
            ----------
            num_of_lattices : int, optional
                Number of random lattices to put together
            max_lattice_order : int, optional
                Defines how much the lattice is resolved as single dots.
            special_moire_symmetry_prob : float, optional
                Most common moiré effect are lines (symmetry=1).
                This parameter specifies probability of more intricate patterns.

            Returns
            -------
            _ : np.array
                Moiré lattice comprised of several lattices
        """

        result = self.create_random_lattice(max_lattice_order, special_moire_symmetry_prob)
        for i in range(0, num_of_lattices - 1):
            result = result + self.create_random_lattice(max_lattice_order, special_moire_symmetry_prob)

        return result.compute()
