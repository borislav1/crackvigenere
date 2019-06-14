
# Introduction


The [Vigenere cipher](https://en.wikipedia.org/wiki/Vigen%C3%A8re_cipher) is a method of encrypting alphabetic text by using multiple [Caesar cipher](https://en.wikipedia.org/wiki/Caesar_cipher)s. If we set key of length one or key with equal shifting factors, we get Caesar cipher. The first step in breaking the cipher is to estimate the length of the key, this can be done by using the [index of coincidence](https://en.wikipedia.org/wiki/Index_of_coincidence). When this is done, we have k Caesar ciphers, where k is the length of the key. The second step is to break the k Caesar ciphers, this can be done by using the [frequency analysis](https://en.wikipedia.org/wiki/Frequency_analysis), for each k-th Caesar cipher.


# Running


The program works only with English letters, any other letter will be treated as punctuation sign. The efficiency of the program directly depends on the size of the cipher text and is it proper English. The program expects path to the cipher text file, as a command line argument, and writes the cracked cipher to the standard output. 











