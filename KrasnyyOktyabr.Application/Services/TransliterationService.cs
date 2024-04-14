﻿using System.Text;

namespace KrasnyyOktyabr.Application.Services;

public class TransliterationService : ITransliterationService
{
    private static readonly Dictionary<char, string> s_cyrillicCharacterMappings = new()
    {
        { 'а', "a" },
        { 'б', "b" },
        { 'в', "v" },
        { 'г', "g" },
        { 'д', "d" },
        { 'е', "e" },
        { 'ё', "yo" },
        { 'ж', "zh"},
        { 'з', "z" },
        { 'и', "i" },
        { 'й', "j" },
        { 'к', "k" },
        { 'л', "l" },
        { 'м', "m" },
        { 'н', "n" },
        { 'о', "o" },
        { 'п', "p" },
        { 'р', "r" },
        { 'с', "s" },
        { 'т', "t" },
        { 'у', "u" },
        { 'ф', "f" },
        { 'х', "h" },
        { 'ц', "c" },
        { 'ч', "ch" },
        { 'ш', "sh" },
        { 'щ', "sch" },
        { 'ъ', "j" },
        { 'ы', "i" },
        { 'ь', "j" },
        { 'э', "e" },
        { 'ю', "yu" },
        { 'я', "ya" },
        { 'А', "A" },
        { 'Б', "B" },
        { 'В', "V" },
        { 'Г', "G" },
        { 'Д', "D" },
        { 'Е', "E" },
        { 'Ё', "Yo" },
        { 'Ж', "Zh" },
        { 'З', "Z" },
        { 'И', "I" },
        { 'Й', "J" },
        { 'К', "K" },
        { 'Л', "L" },
        { 'М', "M" },
        { 'Н', "N" },
        { 'О', "O" },
        { 'П', "P" },
        { 'Р', "R" },
        { 'С', "S" },
        { 'Т', "T" },
        { 'У', "U" },
        { 'Ф', "F" },
        { 'Х', "H" },
        { 'Ц', "C" },
        { 'Ч', "Ch" },
        { 'Ш', "Sh" },
        { 'Щ', "Sch" },
        { 'Ъ', "J" },
        { 'Ы', "I" },
        { 'Ь', "J" },
        { 'Э', "E" },
        { 'Ю', "Yu" },
        { 'Я', "Ya" }
    };

    public string TransliterateToLatin(string source)
    {
        return TransliterateCyrillicToLatin(source);
    }

    private static string TransliterateCyrillicToLatin(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        StringBuilder result = new();

        foreach (char character in source)
        {
            bool canMap = s_cyrillicCharacterMappings.TryGetValue(character, out string? mapped);
            result.Append(canMap ? mapped : character.ToString());
        }

        return result.ToString();
    }
}
