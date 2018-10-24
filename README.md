# GenericContext
Generic DbContext, UnitOfWork and Repository for your EntityFramework projects.

This is a generic context and repository pattern to be implemented on your EntityFramework project.
The goal here is to have generic methods that can easily be inherited by all your repositories. This pattern also allows building multiple contexts for your application, including allowing you to have contexts connected to different databases.
By default, it disables the entity attachment from your database, which can be changed using the method SetTrackingBehavior on the GenericContext class.

To implement this is simple, create a DbContext class, and instead of inheriting from *Microsoft.EntityFrameworkCore.DbContext*, inherit from this project's GenericContext class. Then you must pass the specific DbContext type on it's inheritance, following the example:

```
public class MyContext : GenericContext<MyContext>
```

This context will have to implement the *DatabaseConfig* abstract method, which tells your context how to connect to your database, just like you would have to do by overloading the *OnConfiguring* virtual method from *Microsoft.EntityFrameworkCore.DbContext*.

To use the *GenericRepository* and *GenericUnitofWork* classes you simply must inherit those classes and you will have all their methods available, as the example:

```
public class MyRepository<TEntity> : GenericRepository<TEntity, MyContext>

public MyRepository(MyContext dbContext)
  : base(dbContext as GenericContext<MyContext>)
{
    _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
}
```
From now on, your repository instances will have a full set of CRUD methods (Insert, Update, Delete, Select, etc) already implemented.

This project also uses a pattern where each entity's may do its own mapping, done using a method called **Configure**, as the example:

```
public class MyEntityMapping : IEntityTypeConfiguration<MyEntity>
{
  public void Configure(EntityTypeBuilder<MyEntity> builder)
  { //mapping implementation }
}
```

This pattern replaces using Data Annotations or Fluent Api patterns, as seen on the [official documentation](https://docs.microsoft.com/en-us/ef/core/modeling/relational/columns).
