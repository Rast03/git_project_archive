
public enum Value {
	
Two(2), Three(3), Four(4), Five(5), Six(6), Seven(7), Eight(8), Nine(9),
Ten(10), Jack(11), Queen(12), King(13), Ace(14);
	
	
	//private field
	private final int cardValueInt;
	
	//Constructor
	private Value(int cardValueInt) {
		this.cardValueInt = cardValueInt;
	}
	
	//Method 
	public int getCardValue(){
		return cardValueInt;
	}
}
