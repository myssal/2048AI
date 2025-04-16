using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class AI
{
	public int best_operation = 0;
	public int[] grid = new int[16];
	public int node = 0;
	public int max_depth = 3;

	public int[] MoveLeft(int[] s, out int score)
	{
		int k = 0;
		int baseIdx = 0;
		score = 0;
		int[] result = new int[16];

		for (int i = 4; i <= 16; i += 4)
		{
			while (k < i)
			{
				if (s[k] == 0)
				{
					k++;
					continue;
				}
				if (k + 1 < i && s[k] == s[k + 1])
				{
					result[baseIdx++] = s[k] * 2;
					score += s[k] * 2;
					k += 2;
				}
				else
				{
					result[baseIdx++] = s[k++];
				}
			}
			while (baseIdx < i)
			{
				result[baseIdx++] = 0;
			}
		}
		return result;
	}

	public int[] Rotate(int[] s)
	{
		return new int[]
		{
			s[12], s[8], s[4], s[0],
			s[13], s[9], s[5], s[1],
			s[14], s[10], s[6], s[2],
			s[15], s[11], s[7], s[3]
		};
	}

	public int Estimate(int[] s)
	{
		int diff = 0;
		int sum = 0;
		for (int i = 0; i < 16; ++i)
		{
			sum += s[i];
			if (i % 4 != 3)
				diff += Math.Abs(s[i] - s[i + 1]);
			if (i < 12)
				diff += Math.Abs(s[i] - s[i + 4]);
		}
		return (sum * 4 - diff) * 2;
	}

	public double Search(int[] s, int depth)
	{
		node++;
		if (depth >= max_depth) return Estimate(s);

		double best = -1;
		int[] original = (int[])s.Clone();

		for (int i = 0; i < 4; ++i)
		{
			int score;
			int[] t = MoveLeft(s, out score);
			bool same = true;
			for (int j = 0; j < 16; ++j)
			{
				if (t[j] != s[j])
				{
					same = false;
					break;
				}
			}

			if (!same)
			{
				double temp = 0;
				int emptySlots = 0;

				for (int j = 0; j < 16; ++j)
				{
					if (t[j] == 0)
					{
						t[j] = 2;
						emptySlots++;
						temp += Search(t, depth + 1) * 0.9;
						t[j] = 4;
						temp += Search(t, depth + 1) * 0.1;
						t[j] = 0;
					}
				}

				if (emptySlots != 0)
					temp /= emptySlots;
				else
					temp = -1e20;

				if (score + temp > best)
				{
					best = score + temp;
					if (depth == 0)
					{
						best_operation = i;
					}
				}
			}

			if (i != 3)
				s = Rotate(s);
		}

		return best;
	}

	public void SetTile(int x, int y, int v)
	{
		grid[x + y * 4] = v;
	}

	public void StartSearch()
	{
		node = 0;
		max_depth = 3;
		while (true)
		{
			node = 0;
			Search((int[])grid.Clone(), 0);
			if (node >= 10000 || max_depth >= 8) break;
			max_depth++;
		}
	}
}

