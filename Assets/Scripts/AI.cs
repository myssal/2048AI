using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{
	[SerializeField] private TileBoard board;
	private int[,] grid;
	private int bestOperation;
	private int nodeCount;
	private int maxDepth = 3;
	private const int MAX_NODES = 10000;
	private const float HUGE_PENALTY = -1e20f;

	private void Awake()
	{
		grid = new int[4, 4];
	}

	public void AutoSolve()
	{
		if (board == null) return;
		StartCoroutine(AutoSolveCoroutine());
	}

	private IEnumerator AutoSolveCoroutine()
	{
		while (true)
		{
			UpdateGridState();
			
			// Check if 2048 tile exists
			bool has2048 = false;
			for (int y = 0; y < 4; y++)
			{
				for (int x = 0; x < 4; x++)
				{
					if (grid[x, y] == 2048)
					{
						has2048 = true;
						break;
					}
				}
				if (has2048) break;
			}
			
			if (has2048)
			{
				Debug.Log("2048 tile reached! Auto-solve stopped.");
				break;
			}
			
			StartSearch();
			
			if (bestOperation != -1)
			{
				ExecuteMove(bestOperation);
				yield return new WaitForSeconds(0.04f);
			}
			else
			{
				Debug.Log("No valid moves found");
				break;
			}
		}
	}

	public void SingleStep()
	{
		if (board == null) return;
		UpdateGridState();
		StartSearch();
		if (bestOperation != -1)
		{
			ExecuteMove(bestOperation);
		}
	}

	private void UpdateGridState()
	{
		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 4; x++)
			{
				TileCell cell = board.grid.GetCell(x, y);
				grid[x, y] = cell.Occupied ? cell.tile.state.number : 0;
			}
		}
	}

	private void StartSearch()
	{
		nodeCount = 0;
		maxDepth = 3;
		
		while (true)
		{
			nodeCount = 0;
			Search(grid, 0);
			
			if (nodeCount >= MAX_NODES || maxDepth >= 8)
				break;
				
			maxDepth++;
		}
	}

	private float Search(int[,] currentGrid, int depth)
	{
		nodeCount++;
		
		if (depth >= maxDepth)
			return Estimate(currentGrid);
			
		float bestScore = float.MinValue;
		
		for (int move = 0; move < 4; move++)
		{
			int[,] newGrid = CloneGrid(currentGrid);
			int score = SimulateMove(newGrid, move);
			
			if (score == -1) // Move was invalid
				continue;
				
			int emptyCount = CountEmptyCells(newGrid);
			float expectedScore = 0;
			
			if (emptyCount == 0)
			{
				expectedScore = HUGE_PENALTY;
			}
			else
			{
				for (int y = 0; y < 4; y++)
				{
					for (int x = 0; x < 4; x++)
					{
						if (newGrid[x, y] == 0)
						{
							// Try 2 (90% chance)
							newGrid[x, y] = 2;
							expectedScore += 0.9f * Search(newGrid, depth + 1);
							
							// Try 4 (10% chance)
							newGrid[x, y] = 4;
							expectedScore += 0.1f * Search(newGrid, depth + 1);
							
							newGrid[x, y] = 0;
						}
					}
				}
				expectedScore /= emptyCount;
			}
			
			float totalScore = score + expectedScore;
			if (totalScore > bestScore)
			{
				bestScore = totalScore;
				if (depth == 0)
				{
					bestOperation = move;
				}
			}
		}
		
		return bestScore;
	}

	private float Estimate(int[,] grid)
	{
		float sum = 0;
		float penalty = 0;
		
		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 4; x++)
			{
				sum += grid[x, y];
				
				// Horizontal penalty
				if (x < 3)
				{
					penalty += Mathf.Abs(grid[x, y] - grid[x + 1, y]);
				}
				
				// Vertical penalty
				if (y < 3)
				{
					penalty += Mathf.Abs(grid[x, y] - grid[x, y + 1]);
				}
			}
		}
		
		return (sum * 4 - penalty) * 2;
	}

	private int[,] CloneGrid(int[,] source)
	{
		int[,] clone = new int[4, 4];
		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 4; x++)
			{
				clone[x, y] = source[x, y];
			}
		}
		return clone;
	}

	private int CountEmptyCells(int[,] grid)
	{
		int count = 0;
		for (int y = 0; y < 4; y++)
		{
			for (int x = 0; x < 4; x++)
			{
				if (grid[x, y] == 0) count++;
			}
		}
		return count;
	}

	private int SimulateMove(int[,] grid, int move)
	{
		bool changed = false;
		int score = 0;
		
		switch (move)
		{
			case 0: // Up
				for (int x = 0; x < 4; x++)
				{
					for (int y = 1; y < 4; y++)
					{
						if (grid[x, y] != 0)
						{
							int ny = y;
							while (ny > 0 && (grid[x, ny - 1] == 0 || grid[x, ny - 1] == grid[x, ny]))
							{
								if (grid[x, ny - 1] == grid[x, ny])
								{
									grid[x, ny - 1] *= 2;
									score += grid[x, ny - 1];
									grid[x, ny] = 0;
									changed = true;
									break;
								}
								else if (grid[x, ny - 1] == 0)
								{
									grid[x, ny - 1] = grid[x, ny];
									grid[x, ny] = 0;
									changed = true;
									ny--;
								}
							}
						}
					}
				}
				break;
				
			case 1: // Right
				for (int y = 0; y < 4; y++)
				{
					for (int x = 2; x >= 0; x--)
					{
						if (grid[x, y] != 0)
						{
							int nx = x;
							while (nx < 3 && (grid[nx + 1, y] == 0 || grid[nx + 1, y] == grid[nx, y]))
							{
								if (grid[nx + 1, y] == grid[nx, y])
								{
									grid[nx + 1, y] *= 2;
									score += grid[nx + 1, y];
									grid[nx, y] = 0;
									changed = true;
									break;
								}
								else if (grid[nx + 1, y] == 0)
								{
									grid[nx + 1, y] = grid[nx, y];
									grid[nx, y] = 0;
									changed = true;
									nx++;
								}
							}
						}
					}
				}
				break;
				
			case 2: // Down
				for (int x = 0; x < 4; x++)
				{
					for (int y = 2; y >= 0; y--)
					{
						if (grid[x, y] != 0)
						{
							int ny = y;
							while (ny < 3 && (grid[x, ny + 1] == 0 || grid[x, ny + 1] == grid[x, ny]))
							{
								if (grid[x, ny + 1] == grid[x, ny])
								{
									grid[x, ny + 1] *= 2;
									score += grid[x, ny + 1];
									grid[x, ny] = 0;
									changed = true;
									break;
								}
								else if (grid[x, ny + 1] == 0)
								{
									grid[x, ny + 1] = grid[x, ny];
									grid[x, ny] = 0;
									changed = true;
									ny++;
								}
							}
						}
					}
				}
				break;
				
			case 3: // Left
				for (int y = 0; y < 4; y++)
				{
					for (int x = 1; x < 4; x++)
					{
						if (grid[x, y] != 0)
						{
							int nx = x;
							while (nx > 0 && (grid[nx - 1, y] == 0 || grid[nx - 1, y] == grid[nx, y]))
							{
								if (grid[nx - 1, y] == grid[nx, y])
								{
									grid[nx - 1, y] *= 2;
									score += grid[nx - 1, y];
									grid[nx, y] = 0;
									changed = true;
									break;
								}
								else if (grid[nx - 1, y] == 0)
								{
									grid[nx - 1, y] = grid[nx, y];
									grid[nx, y] = 0;
									changed = true;
									nx--;
								}
							}
						}
					}
				}
				break;
		}
		
		return changed ? score : -1;
	}

	private void ExecuteMove(int move)
	{
		switch (move)
		{
			case 0: // Up
				board.Move(Vector2Int.up, 0, 1, 1, 1);
				break;
			case 1: // Right
				board.Move(Vector2Int.right, board.grid.Width - 2, -1, 0, 1);
				break;
			case 2: // Down
				board.Move(Vector2Int.down, 0, 1, board.grid.Height - 2, -1);
				break;
			case 3: // Left
				board.Move(Vector2Int.left, 1, 1, 0, 1);
				break;
		}
	}
}

