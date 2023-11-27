# File: noise_creator.py
# Solution: [Thesis] Keyboard and Keys Image Recognition
# Year: 2022/2023
# Author: Jan Lorenc
# University: Brno University of Technology
# Faculty: Faculty of Information Technology
# Description: Class capable of various noises (gaussian, poisson, s&p, speckle).

import numpy as np


class NoiseCreator:
    """ Namespace class for noise creation methods """

    @staticmethod
    def add_gaussian_noise(img, min_sigma=0.5, max_sigma=10):
        """ Generates gaussian noise with randomized variance and adds it to the image.

            Parameters
            ----------
            img : np.array
                Image to which the noise should be added
            min_sigma : float, optional
                Minimum boundary for standard deviation
            max_sigma : float, optional
                Maximum boundary for standard deviation

            Returns
            -------
            _ : np.array
                Image with gaussian noise.
        """

        sigma = np.random.uniform(min_sigma, max_sigma)
        gauss = np.random.normal(0, sigma, img.shape)
        gauss = gauss.reshape(*img.shape)
        return img + gauss

    @staticmethod
    def add_salt_and_pepper_noise(img, min_prob=0.005, max_prob=0.01):
        """ Generates salt and pepper noise with randomized probability and adds it to the image.

            Parameters
            ----------
            img : np.array
                Image to which the noise should be added
            min_prob : float, optional
                Minimum boundary for s&p probability of application
            max_prob : float, optional
                Maximum boundary for s&p probability of application

            Returns
            -------
            _ : np.array
                Image with s&p noise.
        """

        shape = img.shape
        probability = np.random.uniform(min_prob, max_prob)
        for i in range(shape[0]):
            for j in range(shape[1]):
                rdn = np.random.random()
                if rdn < probability:
                    img[i][j] = 0
                elif rdn > 1 - probability:
                    img[i][j] = 255

        return img

    @staticmethod
    def add_poisson_noise(img):
        """ Generates poisson noise and adds it to the image.
            This noise can make the image quite light, so it is randomly reduced.

            Parameters
            ----------
            img : np.array
                Image to which the noise should be added

            Returns
            -------
            _ : np.array
                Image with poisson noise.
        """

        reduction = np.random.randint(2, 7)
        img[img < 0] = 0
        return img + np.random.poisson(img) / reduction

    @staticmethod
    def add_speckle_noise(img, min_variance=0.05, max_variance=0.4):
        """ Generates speckle noise with randomized variance and adds it to the image.

            Parameters
            ----------
            img : np.array
                Image to which the noise should be added
            min_variance : float, optional
                Minimum boundary for variance
            max_variance : float, optional
                Maximum boundary for variance

            Returns
            -------
            _ : np.array
                Image with gaussian noise.
        """

        gauss = np.random.randn(*img.shape)
        gauss = gauss.reshape(*img.shape)
        var = np.random.uniform(min_variance, max_variance)
        return img + img * gauss * var

    @staticmethod
    def add_random_noise(img):
        """ Randomly chooses a noise which it adds to the image.

            Parameters
            ----------
            img : np.array
                Image to which the noise should be added

            Returns
            -------
            _ : np.array
                Image with a random noise.
        """

        r = np.random.random()
        if r < 0.25:
            return NoiseCreator.add_gaussian_noise(img)
        elif r < 0.5:
            return NoiseCreator.add_salt_and_pepper_noise(img)
        elif r < 0.75:
            return NoiseCreator.add_poisson_noise(img)
        else:
            return NoiseCreator.add_speckle_noise(img)
