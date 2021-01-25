import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.net.Socket;
import java.util.ArrayList;
import java.util.Scanner;

public class Client {

	public static void main(String[] args) {
		Scanner scan = new Scanner(System.in);

		try {
			System.out.println("Type the IP or name of the server, if on the same machine type: localhost ");
			String ip = scan.nextLine();
			System.out.println("Type the port of the server, the default should be 8000 ");
			int port = Integer.parseInt(scan.nextLine());
			
			Socket socket = new Socket(ip, port);

			DataOutputStream output = new DataOutputStream(socket.getOutputStream());
			DataInputStream input = new DataInputStream(socket.getInputStream());

			System.out.println("Successfully joined the server");
			System.out.print("Type your user name: ");

			output.writeUTF(scan.nextLine());
			output.flush();

			
			System.out.println("You can use commands 'ready' to initiate your game(warning: you can't chat anymore) or command 'quit' to leave the lobby");
			
			Thread write = new Thread(() -> {
				boolean connect = true;
				while (connect) {
					try {
						String message = scan.nextLine();
						output.writeUTF(message);
						output.flush();

						if (message.equalsIgnoreCase("quit")) {
							socket.close();
							connect = false;
							scan.close();
						}

						if (message.equalsIgnoreCase("ready")) {
							connect = false;
//							scan.close();
						}

					} catch (IOException e) {
						e.printStackTrace();
					}
				}

			});
			write.start();

			Thread read = new Thread(() -> {
				boolean connect = true;
				while (connect) {
					try {
						String message = input.readUTF();
						System.out.println(message);
						if (message.equalsIgnoreCase("Game started")) {
							connect = false;
						}
					} catch (IOException e) {
						System.out.println(e + " SocketException expected, do not worry");
						break;
					}
				}

				boolean game = true;
				while(game) {
				boolean actingTurn = false;
				int playersSize = 0;
				ArrayList<Card> userHand = new ArrayList<>();
				try {
					playersSize = input.readInt();
				} catch (IOException e1) {
					e1.printStackTrace();
				}

				for (int round = 1; round <= 4; round++) {
					System.out.println("Round: "+round);
					switch (round) {

					case 1:
						
						for (int i = 0; i < 2; i++) {
							String value = "";
							String suit = "";
							try {
								value = input.readUTF();
								suit = input.readUTF();
							} catch (IOException e) {
								// TODO Auto-generated catch block
								e.printStackTrace();
							}
							userHand.add(new Card(Value.valueOf(value), Suit.valueOf(suit)));
						}

						break;

					case 2:

						for (int i = 0; i < 3; i++) {
							String value = "";
							String suit = "";
							try {
								value = input.readUTF();
								suit = input.readUTF();
							} catch (IOException e) {
								// TODO Auto-generated catch block
								e.printStackTrace();
							}
							userHand.add(new Card(Value.valueOf(value), Suit.valueOf(suit)));
						}
						break;

					case 3:
						String value = "";
						String suit = "";
						try {
							value = input.readUTF();
							suit = input.readUTF();
						} catch (IOException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
						userHand.add(new Card(Value.valueOf(value), Suit.valueOf(suit)));
						break;

					case 4:
						String value1 = "";
						String suit1 = "";
						try {
							value1 = input.readUTF();
							suit1 = input.readUTF();
						} catch (IOException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
						userHand.add(new Card(Value.valueOf(value1), Suit.valueOf(suit1)));
						break;
					}

					int[] value = new int[userHand.size()];
					String[] color = new String[userHand.size()];
					for (int i = 0; i < userHand.size(); i++) {
						value[i] = userHand.get(i).getCardValue().getCardValue();
						color[i] = userHand.get(i).getSuit().printRankCard();
					}

					DeckAscii.printHand(value, color);

					for (int i = 0; i < playersSize; i++) {

						try {
							actingTurn = input.readBoolean();
							if (actingTurn) {
								System.out.println(input.readUTF());

								boolean correctCommand=false;
								do {
								String command = scan.nextLine();

								output.writeUTF(command);
								output.flush();
								if (command.equalsIgnoreCase("raise")) {
									System.out.println("type Integer for how much u wan to raise");
									output.writeInt(scan.nextInt());
								}
								System.out.println(input.readUTF());
								if(command.equalsIgnoreCase("raise")
										||command.equalsIgnoreCase("call")
										||command.equalsIgnoreCase("check")
										||command.equalsIgnoreCase("fold"))
									correctCommand=true;
								}while(!correctCommand);
							} else {
								System.out.println(input.readUTF());
								System.out.println(input.readUTF());
							}
						} catch (IOException e) {
							// TODO Auto-generated catch block
							e.printStackTrace();
						}
					}
				}
				//here should the end of game be resulted
				try {
					System.out.println(input.readUTF());
					//deciding to end game
					System.out.println(input.readUTF());
					
					for (int i = 0; i < playersSize; i++) {
						String temp = input.readUTF();
						System.out.println(temp);
						if(temp.equals("Type READY to continue or anything else to quit")) {
							String ready = scan.nextLine();
							output.writeUTF(ready);
							output.flush();
						}
						game = input.readBoolean();
						if(game == false) {
							break;
						}
						
					}
					
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
				scan.close();

			});
			read.start();


		} catch (

		IOException ex) {
			System.out.println(ex.toString() + '\n');
		}

	}

}
