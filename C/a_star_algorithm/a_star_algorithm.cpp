#include <iostream>
#include <list> 
#include <iterator> 
#include <time.h>		/* time */
#include <stdio.h>      /* printf, NULL */
#include <stdlib.h>     /* srand, rand */

using namespace std;

struct tile {
	int xCoord = 0;		//
	int yCoord = 0;		//
	int price = 0;
	int cheapPrice = 9999999;
	int manhattan = 0;
	int totalPrice = 9999999;
	int cheapX = 0;
	int cheapY = 0;
	bool visited = false;
	bool shortPath = false;
};

const int sizeX = 10;
const int sizeY = 10;
tile grid[sizeX][sizeY];
list <tile> que;

int startX;
int startY;
int goalX;
int goalY;

bool compareTotalPrice(tile a, tile b)
{
	return (a.totalPrice < b.totalPrice);
}

void randomizeStuff() {
	//random 0-9
	srand(time(NULL));
	startX = rand() % sizeX;
	startY = rand() % sizeY;

	goalX = rand() % sizeX;;
	goalY = rand() % sizeY;;

	std::cout << "Start position: " << startX << " " << startY << "\n";
	std::cout << "Goal position: " << goalX << " " << goalY << "\n";

	//calc manhattan 
	std::cout << "Manhattan prices: \n";
	for (int x = 0; x < sizeX; x++)
	{
		
		for (int y = 0; y < sizeY; y++)
		{
			//manhattan heu + ini of x,y Coord
			grid[x][y].xCoord = x;
			grid[x][y].yCoord = y;
			grid[x][y].manhattan = abs(x - goalX) + abs(y - goalY);
			if(grid[x][y].manhattan <= 9)
				std::cout << grid[x][y].manhattan << " |";
			else
				std::cout << grid[x][y].manhattan << "|";
		}
		std::cout << "\n";
	}


	//randomize the prices:
	std::cout << "Just prices: \n";
	for (int x = 0; x < sizeX; x++)
	{
		for (int y = 0; y < sizeY; y++)
		{
			//random price from 1-9
			grid[x][y].price = rand() % 4 + 1;
			std::cout << grid[x][y].price << " |";
		}
		std::cout << "\n";
	}
}

bool checkSides(tile *inQuestion, tile *currento) {
	if (!(inQuestion->visited))
	{
		if (inQuestion->manhattan == 0) {
			//we guccci, needs to be a method so I can return true
			inQuestion->cheapX = currento->xCoord;
			inQuestion->cheapY = currento->yCoord;
			return true;
		}
		else if (inQuestion->cheapPrice > currento->cheapPrice + inQuestion->price) {
			
			inQuestion->cheapPrice = currento->cheapPrice + inQuestion->price;
			inQuestion->totalPrice = inQuestion->cheapPrice + inQuestion->manhattan;
			inQuestion->cheapX = currento->xCoord;
			inQuestion->cheapY = currento->yCoord;
			que.push_back(*inQuestion); 
		}
	}
	return false;
}

bool aStar(tile *currentTile) {

	for (int i = 0; i < 4; i++)
	{
		int vertical = 0;
		int horizontal = 0;
		switch (i)
		{	
		//this setup is prefering to have the start on bottom left side and the goal on top right side
		case 0:	//up
			vertical = currentTile->xCoord - 1;
			horizontal = currentTile->yCoord;
			break;
		case 1: //right
			vertical = currentTile->xCoord;
			horizontal = currentTile->yCoord + 1;
			break;
		case 2: //down
			vertical = currentTile->xCoord + 1;
			horizontal = currentTile->yCoord;
			break;
		case 3: //left
			vertical = currentTile->xCoord;
			horizontal = currentTile->yCoord-1;
			break;
		}

		if ((vertical >= 0 && vertical < sizeX) && (horizontal >= 0 && horizontal < sizeY))
			if (checkSides(&grid[vertical][horizontal], &grid[currentTile->xCoord][currentTile->yCoord])) {
				std::cout << "found the goal" << "\n";
				return true; // found the goal
			}
				
	}
	currentTile->visited = true;
	grid[currentTile->xCoord][currentTile->yCoord].visited = true;

	que.pop_front();
	que.sort(compareTotalPrice);
	//list<tile>::iterator it;
	//for (it = que.begin(); it != que.end(); ++it) {
	//	tile temp = *it;
	//	std::cout << "totalPrice = " << temp.totalPrice << "\n";
	//	std::cout << "x and y = " << temp.xCoord << " " << temp.yCoord << "\n";
	//}
	if (que.empty())
		return false;

	std::cout << "current state: \n";
	for (int x = 0; x < sizeX; x++)
	{
		for (int y = 0; y < sizeY; y++)
		{
			if (x == startX && y == startY) {
				std::cout << " S" << "|";
			}
			else if (x == goalX && y == goalY) {
				std::cout << " G" << "|";
			}
			else if (grid[x][y].totalPrice < 99999 && !grid[x][y].visited) {
				if (grid[x][y].totalPrice <= 9)
				{
					std::cout << grid[x][y].totalPrice << " |";
				}
				else
				{
					std::cout << grid[x][y].totalPrice << "|";
				}
				
			}
			else if (grid[x][y].visited)
			{
				std::cout << " B|";
			}
			else
			{
				std::cout << "  " << "|";
			}

		}
		std::cout << "\n";
	}


	system("pause");
	aStar(&que.front());
	
}




int main()
{
	randomizeStuff();

	//before recursion
	
	que.push_back(grid[startX][startY]);
	grid[startX][startY].cheapPrice = 0;
	//after recursion

	
	aStar(&grid[startX][startY]);
		

	int spx = goalX;
	int spy = goalY;
	while (!(spx == startX && spy == startY)){
		grid[spx][spy].shortPath = true;
		int tempx = grid[spx][spy].cheapX;
		int tempy = grid[spx][spy].cheapY;
		spx = tempx;
		spy = tempy;
	}

	for (int x = 0; x < sizeX; x++)
	{
		for (int y = 0; y < sizeY; y++)
		{
			if (x == startX && y == startY) {
				std::cout << " S" << "|";
			}
			else if (grid[x][y].shortPath) {
				std::cout << " P" << "|";
			}
			else
			{
				std::cout << "  " << "|";
			}

		}
		std::cout << "\n";
	}
	system("pause");
}
