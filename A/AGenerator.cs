using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A;
public class AGenerator
{
	public char[] AllowedCharacters { get; private set; } = new char[] {
		'a', 'A',
		'ä', 'Ä',
		'À', 'à',
		'Á', 'á',
		'Â', 'â',
		'Ã', 'ã',
		'Å', 'å',
		'α', 'Α',
		'Æ','æ',
		'Ǣ', 'ǣ',
		'Ā', 'ā',
		'Ă', 'ă',
		'Ǟ', 'ǟ',
		'Ǡ', 'ǡ',
		'Ą', 'ą',
		'Ǻ', 'ǻ',
		'Ǽ', 'ǽ',
		'Ȁ', 'ȁ',
		'Ȃ', 'ȃ',
		'Ȧ', 'ȧ',
		'Ⱥ',
		'Ḁ', 'ḁ',
		'ẚ',
		'Ạ', 'ạ',
		'Ả', 'ả',
		'Ấ', 'ấ',
		'Ầ', 'ầ',
		'Ẩ', 'ẩ',
		'Ẫ', 'ẫ',
		'Ậ', 'ậ',
		'Ắ', 'ắ',
		'Ằ', 'ằ',
		'Ẳ', 'ẳ',
		'Ẵ', 'ẵ',
		'Ặ', 'ặ',
		'ɐ', 'ɑ',
		'ɒ',
		'Ά', 'ά',
		'Α',
		'α',
		'@', 'ª',
		
		//greek extended
		'ἀ', 'ἁ', 'ἂ', 'ἃ', 'ἄ', 'ἅ', 'ἆ', 'ἇ', 'Ἀ', 'Ἁ', 'Ἂ','Ἃ', 'Ἄ', 'Ἅ', 'Ἆ', 'Ἇ',
		'ὰ', 'ά',
		'ᾀ', 'ᾁ', 'ᾂ', 'ᾃ', 'ᾄ', 'ᾅ', 'ᾆ', 'ᾇ', 'ᾈ','ᾉ', 'ᾊ', 'ᾋ', 'ᾌ', 'ᾍ', 'ᾎ', 'ᾏ',
		'ᾰ','ᾱ','ᾲ','ᾳ','ᾴ', 'ᾶ', 'ᾷ', 'Ᾰ', 'Ᾱ', 'Ὰ', 'Ά', 'ᾼ',
		
		//Cyrillic
		'А',
		'Ӑ', 'ӑ',
		'Ӓ', 'ӓ',
		'Ӕ', 'ӕ',
		//Georgian
		'მ', 'ძ',
		
		//supperscript and subscript
		'ₐ',
		//Currency symbols
		'₳',
		//letter like
		'Å',
		//Mathematical symbols
		'∀', '∂',
		//misc technical
		'⍶', '⍺', '⎀', '⎁', '⎂', '⎃',
		//enclosed alphanumerics
		'⒜', 'Ⓐ', 'ⓐ',
		//Hangul Compatibility Jamo
		'ㅂ', 'ㅃ', 'ㅸ', 'ㅹ',
		//Enclosed CJK Letters and Months
		'㈅', '㉥',

		};


	public const string Template =
@"
               AAA
              AAAAA
             AAAAAAA
            AAAAAAAAA
           AAAAAAAAAAA
          AAAAAAAAAAAAA
         AAAAAAA AAAAAAA
        AAAAAAA   AAAAAAA
       AAAAAAA     AAAAAAA
      AAAAAAAAAAAAAAAAAAAAA
     AAAAAAAAAAAAAAAAAAAAAAA
    AAAAAAAAAAAAAAAAAAAAAAAAA
   AAAAAAA             AAAAAAA
  AAAAAAA               AAAAAAA
 AAAAAAA                 AAAAAAA
AAAAAAA                   AAAAAAA
";


    public bool ValidateString(ReadOnlySpan<char> message)
    {
        foreach (var character in message)
        {
            if (char.IsControl(character) || char.IsWhiteSpace(character) || AllowedCharacters.Contains(character))
            {
                continue;
            }

            return false;

        }
        return true;
    }
	public char GenerateRandomCharacter()
	{
		return AllowedCharacters[Random.Shared.Next(0, AllowedCharacters.Length)];
	}
	public string GenerateRandomString(int length)
	{
		StringBuilder sb = new StringBuilder(length);
		for (int i = 0; i < length; i++)
		{
			sb.Append(GenerateRandomCharacter());
		}
		return sb.ToString();
	}


    public string GenerateAsciiString(bool random = true, bool fixedWithSpace = false)
    {
        var sb = new StringBuilder(Template.Length);
        for (int i = 0; i < Template.Length; i++)
        {
            var character = Template[i];
            if (random && character == 'A')
            {
                sb.Append(GenerateRandomCharacter());
                continue;
			}
			if (fixedWithSpace && character == ' ')
			{
				sb.Append('⠀'); //Special A character long space
				continue;
			}

			sb.Append(character);

        }
        return sb.ToString();
    }

}
