This project is a local search engine, which can work on a given repository and a search query can return a list of most relevant documents.



Involves two step - 
1. Initial step is to index the contents of each documents.
2. Apply vector space model for finding relevancy between documents and list the selected documents.



Process involved in Indexing:
1. Take a file as input.
2. Extract individual words from the files.
3. Remove the Stop words from the extracted list.
4. Perform Stemming on each word in the list.
5. Get the count of each word.
6. Add the word and the number of times it occurs in the respective file to the index.
7. Add the file details to the catalogue of files.



Process involved in Vector-Space model document retrieval - 
1. In Vector-Space model each document is represented as a vector of size(Vocabulary).
2. Each value of the vector corresponds to a separate term (entry in vocabulary). If a term occurs in the document, its value in the vector is non-zero.
3. In our project each term value is entered as its TF-IDF.
4. The relevancy of documents is computed as cosine similarity between the vector representation of the documents.
5. Rank the documents based on the relevancy and list them for the user.