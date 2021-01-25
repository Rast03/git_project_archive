public class DeckAscii {
	static final char SPADES = 'S';
	static final char HEARTS = 'H';
	static final char DIAMONDS = 'D';
	static final char CLUBS = 'C';


	public static void main(String[] args) { 
		int[] value = {11, 5, 3, 7, 14};
		String[] color = {"Hearts", "Hearts", "Hearts", "Diamonds", "Diamonds"};



		printHand(value, color);

	}

	public static void printHand(int[] value, String[] color) {

		String rank[] = new String[value.length];
		char suit[] = new char[color.length];

		for (int c = 0; c < value.length; c++) {
			switch (value[c]) {
			case 14: // ace
				rank[c] = " A";
				break;
			case 2: // two
				rank[c] = " 2";
				break;
			case 3: // three
				rank[c] = " 3";
				break;
			case 4: // four
				rank[c] = " 4";
				break;
			case 5: // five
				rank[c] = " 5";
				break;
			case 6: // six
				rank[c] = " 6";
				break;
			case 7: // seven
				rank[c] = " 7";
				break;
			case 8: // eight
				rank[c] = " 8";
				break;
			case 9: // nine
				rank[c] = " 9";
				break;
			case 10: // ten
				rank[c] = "10";
				break;
			case 11: // jack
				rank[c] = " J";
				break;
			case 12: // queen
				rank[c] = " Q";
				break;
			case 13: // king
				rank[c] = " K";
				break;
			default:
				break;
			}
		}

		for (int c = 0; c < color.length; c++) {
			switch (color[c]) {
			case "Hearts":
				suit[c] = HEARTS;
				break;
			case "Spades":
				suit[c] = SPADES;
				break;
			case "Diamonds":
				suit[c] = DIAMONDS;
				break;
			case "Clubs":
				suit[c] = CLUBS;
				break;
			default:
				break;
			}
		}


		if (value.length > 2 && color.length > 2) {

			System.out.println("Table:");

			// Top of card

			for (int i = 2; i < rank.length; i++) {
				System.out.print("┌─────");
			}
			System.out.println("───┐");


			// Middle of card

			for (int i = 2; i < rank.length; i++) {
				System.out.print("│" + rank[i] + " " + suit[i] + " ");
			}
			System.out.println("· ·│");

			for (int line = 0; line < 3; line++) {
				for (int i = 2; i < rank.length; i++) {
					System.out.print("│ · · ");
				}
				System.out.println("· ·│");
			}
			for (int i = 2; i < rank.length - 1; i++) {
				System.out.print("│ · · ");
			}
			System.out.println("│ · ·" + rank[rank.length - 1] + " " + suit[suit.length - 1] + "│");

			// Bottom of card

			for (int i = 2; i < rank.length; i++) {
				System.out.print("└─────");
			}
			System.out.println("───┘");

			System.out.println("Hand:");

			// Top of card

			for (int i = 0; i < 2; i++) {
				System.out.print("┌─────");
			}
			System.out.println("───┐");

			// Middle of card

			for (int i = 0; i < 2; i++) {
				System.out.print("│" + rank[i] + " " + suit[i] + " ");
			}
			System.out.println("· ·│");

			for (int line = 0; line < 3; line++) {
				for (int i = 0; i < 2; i++) {
					System.out.print("│ · · ");
				}
				System.out.println("· ·│");
			}
			for (int i = 0; i < 2 - 1; i++) {
				System.out.print("│ · · ");
			}
			System.out.println("│ · ·" + rank[2 - 1] + " " + suit[2 - 1] + "│");

			// Bottom of card

			for (int i = 0; i < 2; i++) {
				System.out.print("└─────");
			}
			System.out.println("───┘");

		} else {

			System.out.println("Hand:");

			// Top of card

			for (int i = 0; i < rank.length; i++) {
				System.out.print("┌─────");
			}
			System.out.println("───┐");

			// Middle of card

			for (int i = 0; i < rank.length; i++) {
				System.out.print("│" + rank[i] + " " + suit[i] + " ");
			}
			System.out.println("· ·│");

			for (int line = 0; line < 3; line++) {
				for (int i = 0; i < rank.length; i++) {
					System.out.print("│ · · ");
				}
				System.out.println("· ·│");
			}
			for (int i = 0; i < rank.length - 1; i++) {
				System.out.print("│ · · ");
			}
			System.out.println("│ · ·" + rank[rank.length - 1] + " " + suit[suit.length - 1] + "│");

			// Bottom of card

			for (int i = 0; i < rank.length; i++) {
				System.out.print("└─────");
			}
			System.out.println("───┘");

		}
	}
}
