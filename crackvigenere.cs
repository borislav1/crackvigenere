using static System.Console;
using System;
using System.Text;

class Helper{
			
	public static bool isEnglishLetter(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
	
	/*helper function which returns UNICODE number for letter a or A
	which is useful for calculating indexes*/
	public static int indexOfLetter(char c) => char.IsUpper(c) ? 65 : 97;
		
	public static int[] countFreq(string text){ //count occurences of every letter in the text
		int[] observedFreq = new int[26];
		foreach(char c in text)
			if (isEnglishLetter(c))//just English letters are counted
				observedFreq[c-indexOfLetter(c)]++;	//subtract UNICODE of A or a so the range will be always between 0-25										
	
		return observedFreq;
	}	
}


class Caesar : Helper{
		
	/*based on the expected frequencies calculate chi square
	 the lowest statistic gives the most probable shift factor with which was shifted the text */	  
	static double chiSquared(int[] observedFreq, double[] expectedFreq){
		double chi = 0;
		int sum = 0;
		foreach(int freq in observedFreq)
			sum += freq;
		for (int i = 0; i < 26; i++)
			chi += Math.Pow(((1.0*observedFreq[i]/sum) - (expectedFreq[i]/100.0)), 2.0)/(expectedFreq[i]/100.0);		
	
		return chi;
	}

	public static string caesarDecrypt(string cipherText){
		double[] expectedFreq = {8.2, 1.5, 2.8, 4.3, 12.7, 2.2, 2.0,6.1, 7.0, 0.2, 0.8, 4.0, 2.4, 6.7,7.5, 1.9, 0.1, 6.0, 6.3, 9.1, 2.8,1.0, 2.4, 0.2, 2.0, 0.1};		
		int[] observedFreq = countFreq(cipherText); //count frequencies of the letters in the cipher
		double[] chiStatistics = new double[26];
		
		for (int i = 0; i < 26; i++) //calculate chi statistic for every amount of shifting		
 			chiStatistics[i] = chiSquared(countFreq(caesarEncrypt(cipherText, i)), expectedFreq);
				
		double min = chiStatistics[0]; //initial smallest chi statistic
		int index = 0;	               //index of the initial smallest chi statistic   
		for (int i = 1; i < chiStatistics.Length; i++){ //find the index of the smallest chi statisic
			if (min > chiStatistics[i]){
				min = chiStatistics[i];
				index = i;
			}
		}
		return caesarEncrypt(cipherText, index); //shift the cipher with the most probable shifting factor to get plain text
	}
	
	public static string caesarEncrypt(string plainText, int factor){
		StringBuilder cipherText = new StringBuilder();
		foreach(char c in plainText){
			if(!isEnglishLetter(c)) //if the character it's not English letter is added to the text
				cipherText.Append(c);
			else
				cipherText.Append((char) ((c+factor-indexOfLetter(c))%26 + indexOfLetter(c))); //shift the letter with the given factor																
		}
		return cipherText.ToString();
	}
}


class Vigenere : Helper{
		
	static string vigenereEncrypt(string plainText, string key){
		string[] cipherParts = split(plainText, key.Length); //split in k parts
		for (int i = 0; i < key.Length; i++) //encrypt every part with appropriate shifting factor
			cipherParts[i] = Caesar.caesarEncrypt(cipherParts[i], key[i] - indexOfLetter(key[i]));
		
		return mergeParts(cipherParts); //merge the encrypted parts
	}
	
	static double indexOfCoincidence(string column){
		int[] freq = countFreq(column);
		int total = 0;
		int sum = 0;
		for (int i = 0; i < 26; i++){
			sum += freq[i]*(freq[i]-1);
			total += freq[i];
		}
		return (double) sum / ((total* (total-1)) / 26);
	}
	
	static int estimateKeyLength(string cipherText){
		double[] delta = new double[11];
		int len = 1;
		for (int keyLength = 1; keyLength <= 10; keyLength++){ //try key lengths from 1 to 10
			string[] parts = split(cipherText, keyLength);
			double sum = 0;
			for (int k = 0; k < keyLength; k++)
				sum += indexOfCoincidence(parts[k]); //index of c. for every column			
			delta[keyLength] = (double) sum / keyLength; //index of c. is average from all columns
		}
		for (int keyLength = 1; keyLength <= 10; keyLength++){
			if (Math.Abs(delta[keyLength]-1.73) <= 0.25){ //the first key length which has margin of 0.25 from 1.73 is the estimation 
				len = keyLength;
				break;
			}
		}
		return len;
	}
		
	/*split the text in k parts where k is the estimated length of the key
	 so for each part separately could be find the amount of shifting 
	 */
	static string[] split(string plainText, int keyLength){ 
		StringBuilder[] parts = new StringBuilder[keyLength];
		for (int i = 0; i < keyLength; i++)
			parts[i] = new StringBuilder();
		int counter = 0; //counter that tells which character is next from the text
		int index = 0; //index that tells in which part will be added the current character
		while(counter < plainText.Length){ //go for every character in the text
			char c = plainText[counter];
			parts[index%keyLength].Append(plainText[counter]); //add character to the appropriate part			
			if(isEnglishLetter(c)) //if the current character is English letter the next character will go in the next part			
				index++;
			counter++;			
		}
		string[] partsStr = new string[keyLength];
		for(int i = 0; i < keyLength; i++)
			partsStr[i] = parts[i].ToString();
	
		return partsStr;
	}
	
	static string mergeParts(string[] parts){ //reverse to the split method
		StringBuilder plainText = new StringBuilder();
		int empty = 0; //how many parts are already empty
		int index = 0; //which part should be used
		
		while(true){			
			plainText.Append(parts[index%parts.Length][0]); //add the first character to the text
			char c = parts[index%parts.Length][0];			
			parts[index%parts.Length] = parts[index%parts.Length].Substring(1); //cut the first character from that part
			
			if (parts[index%parts.Length].Equals("")) empty++; //if the current part is empty/added to the text
			if(empty == parts.Length) break; //if all parts are empty/merged is done
			
			//if the last added character was letter, the next character should be from the next part
			if (isEnglishLetter(c)){ 
				while(true){ //find the next part which is not empty
				index++;
				if(!parts[index%parts.Length].Equals(""))break;
				}
			}										
		}
		return plainText.ToString();		
	}
	
	static string vigenereDecrypt(string cipherText){
		//estimate k the length of the key and split the text in k parts
		int k = estimateKeyLength(cipherText);
		WriteLine("Estimated key length: " + k);
		string[] parts = split(cipherText, k);
		for(int i = 0; i < parts.Length; i++) //decrypt every part separately
			parts[i] = Caesar.caesarDecrypt(parts[i]);
					
		return mergeParts(parts); //merge decrypted parts	
	}
	
	static public int Main(string[] args){
		
		if (args.Length == 0){
			WriteLine("Enter a path to the text file.");
			return 1;
		}
		
		string cipherText = System.IO.File.ReadAllText(args[0]);		
		string crackedText = vigenereDecrypt(cipherText);
		WriteLine(crackedText);
		
		return 0;	
	}
}
