using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<SongDb>(options=>
options.UseSqlite("Data Source=SongDb.db")
);



var app = builder.Build();


app.MapGet("/", () => "Welcome to this");

//GET
app.MapGet("/songs", async(SongDb db)=>
await db.Songs.ToListAsync());




//POST
app.MapPost("/songs", async(Song song, SongDb db)=>
{
    db.Songs.Add(song);
    await db.SaveChangesAsync();

    return Results.Created("Song added:", song);
});


//GET BY ID
app.MapGet("/songs/{id}", async(int id, SongDb db)=>

    await db.Songs.FindAsync(id)
    is Song song
    ? Results.Ok(song)
    : Results.NotFound()
);

//DELETE
app.MapDelete("/songs/{id}", async(int id, SongDb db)=>

    {
    var existingSong = await db.Songs.FindAsync(id);
    if (existingSong == null)
    {
        return Results.NotFound($"Song with ID {id} not found.");
    }

    db.Songs.Remove(existingSong);
    await db.SaveChangesAsync();

    return Results.Ok($"Song with ID {id} deleted successfully.");
});


// PUT
app.MapPut("/songs/{id}", async (int id, Song updatedSong, SongDb db) =>
{
    var existingSong = await db.Songs.FindAsync(id);
    if (existingSong == null)
    {
        return Results.NotFound($"Song with ID {id} not found.");
    }

    existingSong.Title = updatedSong.Title;
    existingSong.Artist = updatedSong.Artist;
    existingSong.Category = updatedSong.Category;
    existingSong.Length = updatedSong.Length;

    await db.SaveChangesAsync();

    return Results.Ok($"Song with ID {id} updated successfully.");
});










app.Run();

public class Song{
        public int Id {get; set;}
        public string Artist {get; set;}

        public string Title {get; set;}
        public int Length {get; set;}

        public string Category {get; set;}
}

class SongDb : DbContext{
    public SongDb(DbContextOptions<SongDb>options)
    :base(options){

    }
    public DbSet <Song> Songs => Set <Song>();
}