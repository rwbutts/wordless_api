namespace WordlessAPI 
{
     static class Filter 
     {

          static bool matchesYellow( string testWord, string guessChar, int charIndex )
          {
               return !matchesGreen( testWord, guessChar, charIndex) && testWord.Contains(guessChar);
          }

          static bool matchesGreen( string testWord, string guessChar, int charIndex )
          {
               return guessChar == testWord.Substring( charIndex, 1 );
          }

          static bool isCompatibleWithClues( string testWord, string answer, string guess )
          {

               for( int i = 0; i < testWord.Length; i++ )
               {
                    string c = guess.Substring( i, 1 );
                    if( matchesGreen( answer, c, i ) )
                    {
                         // guess character matched green against answer, 
                         // but doesnt match green for testWord letter: eliminate
                         if( !matchesGreen( testWord, c, i ) )
                              return false;
                    }
                    else if( matchesYellow ( answer, c, i ) )
                    {
                         // guess matches a letter elsewhere in answer, 
                         // does it do so in testWord?
                         if( !matchesYellow( testWord, c, i ) )
                              return false;
                    }
                    else
                    {
                         // letter c is not in answer. Is it in the Candidate? 
                         // Reject if it is.
                         if( testWord.Contains( c ) )
                              return false;
                    }
               }
               return true;
          }

          public static int findMatches( string[] candidates, string answer, List<string> guesses )
          {
               int matchCount = 0;

               foreach( var candidate in candidates )
               {
                    bool finalIsMatch = true;
                    foreach( var guess in guesses )
                    {
                         if( !isCompatibleWithClues( candidate, answer, guess ) )
                         {
                              finalIsMatch = false;
                              break;
                         }
                    }

                    if( finalIsMatch )
                    {
                         matchCount ++;
                    }
               }

               return matchCount;
          }
     }
}
