﻿namespace Tactical.Monopoly.Queries.EF.Models;

public class Board
{
    public Guid Id { get; set; }

    public DateTime GameStartTime { get; set; }

    public virtual ICollection<BoardScore> BoardScores { get; set; } = new List<BoardScore>();

    public virtual ICollection<Cell> Cells { get; set; } = new List<Cell>();
}
