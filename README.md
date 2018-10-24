# GenericContext
Generic DbContext, UnitOfWork and Repository for your EntityFramework projects.

This is a generic context and repository pattern to be implemented on your EntityFramework project.
The goal here is to have generic methods that can easily be inherited by all your repositories. This pattern also allows building multiple contexts for your application, including allowing you to have contexts connected to different databases.
By default, it disables the entity attachment from your database, which can be changed using the method SetTrackingBehavior on the GenericContext class.

To implement this is simple, create a DbContext class, and instead of inheriting from Microsoft.EntityFrameworkCore.DbContext, inherit from this project GenericContext class, you must pass the specific DbContext type on it's inheritance, following the example:

public class MyContext : GenericContext<MyContext>

The GenericRepository and GenericUnitofWork simply must inherit those classes and you will have all their methods available, as the example:

 public class MyRepository<TEntity> : GenericRepository<TEntity, MyContext>
