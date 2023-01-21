package com.fit.vut.Library.application;

import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.PBEKeySpec;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.security.spec.InvalidKeySpecException;
import java.util.Arrays;
import java.util.Base64;

public class PasswordEncryptor {
    private static final SecureRandom Random = new SecureRandom();
    private static final String Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private static final int Iterations = 10000;
    private static final int KeyLength = 256;
    private static final int SaltLength = 32;

    public static boolean verify(String password, String encryptedPassword) {
        byte[] decodedEncryptedPassword = Base64.getDecoder().decode(encryptedPassword);
        byte[] salt = extractSalt(decodedEncryptedPassword);
        String hashedPassword = encrypt(password, salt);
        return hashedPassword.equalsIgnoreCase(encryptedPassword);
    }

    public static String encrypt(String password){
        byte[] salt = createSalt();
        byte[] hashWithSalt = hash(password, salt);
        return Base64.getEncoder().encodeToString(hashWithSalt);
    }

    private static String encrypt(String password, byte[] salt) {
        byte[] hashWithSalt = hash(password, salt);
        return Base64.getEncoder().encodeToString(hashWithSalt);
    }

    private static byte[] hash(String password, byte[] salt){
        PBEKeySpec spec = new PBEKeySpec(password.toCharArray(), salt, Iterations, KeyLength);
        Arrays.fill(password.toCharArray(), Character.MIN_VALUE);
        try {
            SecretKeyFactory skf = SecretKeyFactory.getInstance("PBKDF2WithHmacSHA1");
            byte[] hash = skf.generateSecret(spec).getEncoded();
            return joinHashAndSalt(hash, salt);
        }
        catch (NoSuchAlgorithmException | InvalidKeySpecException e) {
            throw new AssertionError("Failed to hash a password: " + e.getMessage(), e);
        }
        finally {
            spec.clearPassword();
        }
    }

    private static byte[] createSalt() {
        StringBuilder salt = new StringBuilder(SaltLength);
        for (int i = 0; i < SaltLength; i++)
            salt.append(Alphabet.charAt(Random.nextInt(Alphabet.length())));

        return salt.toString().getBytes();
    }

    private static byte[] joinHashAndSalt(byte[] hash, byte[] salt) {
        byte[] result = new byte[hash.length + salt.length];
        for (int i = 0; i < result.length; i++)
            result[i] = i < hash.length ? hash[i] : salt[i - hash.length];

        return result;
    }

    private static byte[] extractSalt(byte[] combined) {
        byte[] result = new byte[SaltLength];
        System.arraycopy(combined, 32, result, 0, result.length);

        return result;
    }
}
