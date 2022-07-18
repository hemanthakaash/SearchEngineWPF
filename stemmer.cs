using System;
using SearchEngine;
using System.Collections.Generic;
namespace SearchEngine {
	class porterStemmer {
		List<char> word;
		int length;
		int[] m = new int[100];
		bool isVowel(int index) {
			switch (word[index]) {
				case 'A':
				case 'E':
				case 'I':
				case 'O':
				case 'U':
					return true;
				case 'Y':
					if (index == 0) {
						return false;
					}
					return !isVowel(index - 1);
				default:
					return false;
			}
		}
		int findM(int len) {
			if (len < 0) {
				return -1;
			}
			if (m[len] != -1) {
				return m[len];
			}
			int mx = 0;
			bool flag = false;
			for (int i = 0; i < len && mx < 2; i++) {
				if (!isVowel(i)) {
					if (flag) {
						mx++;
						flag = false;
					}
				}
				else {
					flag = true;
				}
			}
			return m[len] = mx;
		}
		bool checkCondition(char c, int len) {
			switch (c) {
				case 'v':
					for (int i = 0; i < len; i++)
						if (isVowel(i))
							return true;
					return false;
				case 'd':
					if (word[length - 1] == word[length - 2]) {
						if (!isVowel(length - 1))
							return true;
					}
					return false;
				case 'o':
					if (len > 2 && !isVowel(len - 3) && isVowel(len - 2) && !isVowel(len - 1)) {
						if (word[len - 1] != 'W' && word[len - 1] != 'X' && word[len - 1] != 'Y') {
							return true;
						}
						return false;
					}	
					return false;
				default:
					return false;
			}
		}
		void removeSuffix(int suf) {
			word[length - suf] = '\0';
			length -= suf;
		}
		void step1a() {
			if ((length > 3 && word[length - 4] == 'S' && word[length - 3] == 'S' && word[length - 2] == 'E' && word[length - 1] == 'S') ||
				(length > 2 && word[length - 3] == 'I' && word[length - 2] == 'E' && word[length - 1] == 'S')) {
				removeSuffix(2);
				return;
			}
			if (length > 1 && word[length - 2] != 'S' && word[length - 1] == 'S') {
				removeSuffix(1);
				return;
			}
		}
		void step1b() {
			bool flag = false;
			if (length > 2 && word[length - 3] == 'E' && word[length - 2] == 'E' && word[length - 1] == 'D' && findM(length - 3) > 0) {
				removeSuffix(1);
				return;
			}
			if (length > 1 && word[length - 2] == 'E' && word[length - 1] == 'D' && checkCondition('v', length - 2)) {
				removeSuffix(2);
				flag = true;
			}
			else if (length > 2 && word[length - 3] == 'I' && word[length - 2] == 'N' && word[length - 1] == 'G' && checkCondition('v', length - 3)) {
				removeSuffix(3);
				flag = true;
			}
			if (flag && length > 1 && ((word[length - 2] == 'A' && word[length - 1] == 'T') ||
				(word[length - 2] == 'B' && word[length - 1] == 'L') ||
				(word[length - 2] == 'I' && word[length - 1] == 'Z') ||
				(findM(length) == 1 && checkCondition('o', length)))) {
				if (word.Count==length) {
					word.Add('E');
				}
                else {
					word[length] = 'E';
                }
				length++;
				return;
			}
			if (flag && length > 1 && word[length - 1] != 'L' && word[length - 1] != 'S' && word[length - 1] != 'Z' && checkCondition('d', length)) {
				removeSuffix(1);
				return;
			}
		}
		void step1c() {
			if (length>0 && word[length - 1] == 'Y' && checkCondition('v', length - 1)) {
				word[length - 1] = 'I';
			}
		}
		void step2() {
			if (findM(length - 7) > 0) {
				if ((word[length - 7] == 'A' && word[length - 6] == 'T' && word[length - 5] == 'I' && word[length - 4] == 'O' && word[length - 3] == 'N' && word[length - 2] == 'A' && word[length - 1] == 'L') ||
					(word[length - 7] == 'I' && word[length - 6] == 'Z' && word[length - 5] == 'A' && word[length - 4] == 'T' && word[length - 3] == 'I' && word[length - 2] == 'O' && word[length - 1] == 'N')) {
					word[length - 5] = 'E';
					removeSuffix(4);
					return;
				}
				if ((word[length - 7] == 'I' && word[length - 6] == 'V' && word[length - 5] == 'E' && word[length - 4] == 'N' && word[length - 3] == 'E' && word[length - 2] == 'S' && word[length - 1] == 'S') ||
					(word[length - 7] == 'F' && word[length - 6] == 'U' && word[length - 5] == 'L' && word[length - 4] == 'N' && word[length - 3] == 'E' && word[length - 2] == 'S' && word[length - 1] == 'S') ||
					(word[length - 7] == 'O' && word[length - 6] == 'U' && word[length - 5] == 'S' && word[length - 4] == 'N' && word[length - 3] == 'E' && word[length - 2] == 'S' && word[length - 1] == 'S')) {
					removeSuffix(4);
					return;
				}
			}
			if (findM(length - 6) > 0) {
				if (word[length - 6] == 'T' && word[length - 5] == 'I' && word[length - 4] == 'O' && word[length - 3] == 'N' && word[length - 2] == 'A' && word[length - 1] == 'L') {
					removeSuffix(2);
					return;
				}
				if (word[length - 6] == 'B' && word[length - 5] == 'I' && word[length - 4] == 'L' && word[length - 3] == 'I' && word[length - 2] == 'T' && word[length - 1] == 'I') {
					word[length - 5] = 'L';
					word[length - 4] = 'E';
					removeSuffix(3);
					return;
				}
			}
			if (findM(length - 5) > 0) {
				if ((word[length - 5] == 'E' && word[length - 4] == 'N' && word[length - 3] == 'T' && word[length - 2] == 'L' && word[length - 1] == 'I') ||
					(word[length - 5] == 'O' && word[length - 4] == 'U' && word[length - 3] == 'S' && word[length - 2] == 'L' && word[length - 1] == 'I')) {
					removeSuffix(2);
					return;
				}
				if ((word[length - 5] == 'A' && word[length - 4] == 'T' && word[length - 3] == 'I' && word[length - 2] == 'O' && word[length - 1] == 'N') ||
					(word[length - 5] == 'I' && word[length - 4] == 'V' && word[length - 3] == 'I' && word[length - 2] == 'V' && word[length - 1] == 'I')) {
					word[length - 3] = 'E';
					removeSuffix(2);
					return;
				}
				if ((word[length - 5] == 'A' && word[length - 4] == 'L' && word[length - 3] == 'I' && word[length - 2] == 'S' && word[length - 1] == 'M') ||
					(word[length - 5] == 'A' && word[length - 4] == 'L' && word[length - 3] == 'I' && word[length - 2] == 'T' && word[length - 1] == 'I')) {
					removeSuffix(3);
					return;
				}
			}
			if (findM(length - 4) > 0) {
				if (((word[length - 4] == 'E' || word[length - 4] == 'A') && word[length - 3] == 'N' && word[length - 2] == 'C' && word[length - 1] == 'I') ||
					(word[length - 4] == 'A' && word[length - 3] == 'B' && word[length - 2] == 'L' && word[length - 1] == 'I')) {
					word[length - 1] = 'E';
					return;
				}
				if (word[length - 4] == 'I' && word[length - 3] == 'Z' && word[length - 2] == 'E' && word[length - 1] == 'R') {
					removeSuffix(1);
					return;
				}
				if (word[length - 4] == 'A' && word[length - 3] == 'L' && word[length - 2] == 'L' && word[length - 1] == 'I') {
					removeSuffix(2);
					return;
				}
				if (word[length - 4] == 'A' && word[length - 3] == 'T' && word[length - 2] == 'O' && word[length - 1] == 'R') {
					word[length - 2] = 'E';
					removeSuffix(1);
					return;
				}
			}
			if (findM(length - 3) > 0) {
				if (word[length - 3] == 'E' && word[length - 2] == 'L' && word[length - 1] == 'I') {
					removeSuffix(2);
					return;
				}
			}
		}
		void step3() {
			if (findM(length - 5) > 0) {
				if ((word[length - 5] == 'I' && word[length - 4] == 'C' && word[length - 3] == 'A' && word[length - 2] == 'T' && word[length - 1] == 'E') ||
					(word[length - 5] == 'A' && word[length - 4] == 'L' && word[length - 3] == 'I' && word[length - 2] == 'Z' && word[length - 1] == 'E') ||
					(word[length - 5] == 'I' && word[length - 4] == 'C' && word[length - 3] == 'I' && word[length - 2] == 'T' && word[length - 1] == 'I')) {
					removeSuffix(3);
					return;
				}
				if (word[length - 5] == 'A' && word[length - 4] == 'T' && word[length - 3] == 'I' && word[length - 2] == 'V' && word[length - 1] == 'E') {
					removeSuffix(5);
					return;
				}
			}
			if (findM(length - 3) > 0) {
				if (word[length - 3] == 'F' && word[length - 2] == 'U' && word[length - 1] == 'L') {
					removeSuffix(3);
					return;
				}
			}
			if (findM(length - 4) > 0) {
				if (word[length - 4] == 'I' && word[length - 3] == 'C' && word[length - 2] == 'A' && word[length - 1] == 'L') {
					removeSuffix(2);
					return;
				}
				if (word[length - 4] == 'N' && word[length - 3] == 'E' && word[length - 2] == 'S' && word[length - 1] == 'S') {
					removeSuffix(4);
					return;
				}
			}
		}
		void step4() {
			if (findM(length - 5) > 1) {
				if (word[length - 5] == 'E' && word[length - 4] == 'M' && word[length - 3] == 'E' && word[length - 2] == 'N' && word[length - 1] == 'T') {
					removeSuffix(5);
					return;
				}
			}
			if (findM(length - 3) > 1) {
				if ((word[length - 3] == 'I' && word[length - 2] == 'S' && word[length - 1] == 'M') ||
					(word[length - 3] == 'A' && word[length - 2] == 'T' && word[length - 1] == 'E') ||
					(word[length - 3] == 'I' && word[length - 2] == 'T' && word[length - 1] == 'I') ||
					(word[length - 3] == 'O' && word[length - 2] == 'U' && word[length - 1] == 'S') ||
					(word[length - 3] == 'I' && word[length - 2] == 'Z' && word[length - 1] == 'E') ||
					(word[length - 3] == 'I' && word[length - 2] == 'V' && word[length - 1] == 'E') ||
					(word[length - 3] == 'A' && word[length - 2] == 'N' && word[length - 1] == 'T') ||
					(word[length - 3] == 'E' && word[length - 2] == 'N' && word[length - 1] == 'T') ||
					((word[length - 4] == 'S' || word[length - 4] == 'T') && word[length - 3] == 'I' && word[length - 2] == 'O' && word[length - 1] == 'N')) {
					removeSuffix(3);
					return;
				}
			}
			if (findM(length - 4) > 1) {
				if ((word[length - 4] == 'A' && word[length - 3] == 'N' && word[length - 2] == 'C' && word[length - 1] == 'E') ||
					(word[length - 4] == 'E' && word[length - 3] == 'N' && word[length - 2] == 'C' && word[length - 1] == 'E') ||
					(word[length - 4] == 'I' && word[length - 3] == 'B' && word[length - 2] == 'L' && word[length - 1] == 'E') ||
					(word[length - 4] == 'A' && word[length - 3] == 'B' && word[length - 2] == 'L' && word[length - 1] == 'E') ||
					(word[length - 4] == 'M' && word[length - 3] == 'E' && word[length - 2] == 'N' && word[length - 1] == 'T')) {
					removeSuffix(4);
					return;
				}
			}
			if (length > 1 && ((word[length - 2] == 'A' && word[length - 1] == 'L') ||
				(word[length - 2] == 'E' && word[length - 1] == 'R') ||
				(word[length - 2] == 'I' && word[length - 1] == 'C') ||
				(word[length - 2] == 'O' && word[length - 1] == 'U'))) {
				if (findM(length - 2) > 1) {
					removeSuffix(2);
					return;
				}
			}
		}
		void step5a() {
			int mx;
			if (word[length - 1] == 'E') {
				mx = findM(length - 1);
				if ((mx > 1) || ((mx == 1) && (!checkCondition('o', length - 1)))) {
					removeSuffix(1);
				}
			}
		}
		void step5b() {
			if (length > 1 && word[length - 1] == 'L' && word[length - 2] == 'L' && findM(length) > 1) {
				removeSuffix(1);
			}
		}
		public string stem(string w) {
			word = new List<char>(w.ToCharArray());
			length = word.Count;
			for (int i = 0; i < length; m[i++] = -1) ;
			step1a();
			step1b();
			step1c();
			step2();
			step3();
			step4();
			step5a();
			step5b();
			w = "";
			for (int i = 0; i < length; i++) {
				w += word[i].ToString();
			}
			return w;
		}
	}
}