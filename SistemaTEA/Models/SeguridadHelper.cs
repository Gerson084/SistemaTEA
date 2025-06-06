using System;
using System.Security.Cryptography;

public static class SeguridadHelper
{
    public static string HashPassword(string password)
    {
        // Generar salt de 16 bytes
        byte[] salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        // Derivar el hash con PBKDF2
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Combinar salt + hash en un solo arreglo
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerificarPassword(string textoPlano, string hashAlmacenado)
    {
        byte[] hashBytes = Convert.FromBase64String(hashAlmacenado);

        // Extraer el salt de los primeros 16 bytes
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        // Volver a calcular el hash con la contraseña proporcionada y el salt extraído
        var pbkdf2 = new Rfc2898DeriveBytes(textoPlano, salt, 10000);
        byte[] hashCalculado = pbkdf2.GetBytes(20);

        // Comparar byte a byte con el hash original
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hashCalculado[i])
                return false;
        }

        return true;
    }
}

