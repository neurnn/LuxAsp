## LuxAsp.
The Web Framework based on ASP.Net Core.

## Defining the Application
```
public class Program : LuxHostApplication {
	public static void Main(string[] args) => Run<Program>(args);
	protected override void Configure(ILuxHostBuilder Builder, string[] Arguments) {
		/* Configure the Application here. */
	}
}
```
Users can configure the application using `Builder` instance.

## Configuring the Database
```
Builder.ConfigureDatabase(Options => {
	Options
		.Setup((Configuration, DbOptions) => {
			var ConnString = Configuration.GetValue("ConnString", "");
			
			if (string.IsNullOrWhiteSpace(ConnString))
				throw new InvalidProgramException();
			
			DbOptions.UseMySQL(ConnString, 
				X => X.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
		});
});
```

### Using the Repository Pattern.
First, define a model.
```
class MyModel : DataModel { /*: LuxAsp.EFCore.DataModel */
	[Key] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }
	
	[UniversalDateTime]
	public DateTime CreationTime { get; set; } = DateTime.Now;

	/* ... blah blah ... */
}
```

Second, define a model's repository. (Optional)
```
class MyModelRepository : Repository<MyModel> {
	/* Called for saving the model, real saving performed by `Next` delegate. */
	protected override Task OnSaveRequest(DeviceProperty Entity, Func<Task<bool>> Next)
	{
		if (Entity.IsNew)
			Entity.CreationTime = DateTime.Now;

		return base.OnSaveRequest(Entity, Next);
	}
	
	/* ... make utility functions for the model manipulation. */
}
```

Third, register them to database.
```
Builder.ConfigureDatabase(Options => {
	Options.With<MyModel, MyModelRepository>(); // --> with custom repository.
	Options.With<MyModel>(); // -- with default repository implementation.
};
```

Then, refer the repository through dependency injection.
```
public class MyPageModel : PageModel {
	public IAsyncResult OnGet([FromServices] MyModelRepository MyModels) {
		/* ... blah blah ... */
		
		return Page();
	}
}
```

Note that, when default repository implementation specified, it can be refered by `Repository<MyModel>`.

### Using the Advanced Session System.