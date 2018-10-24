using GenericContext.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace GenericContext.Context
{
    /// <summary>
    /// Generic DbContext to implement standard methods.
    /// </summary>
    /// <typeparam name="TContext">Context type.</typeparam>
    public abstract class GenericContext<TContext> : DbContext
    {
        #region Abstract methods

        #region Database configuration

        /// <summary>
        /// Abstract method to configure implement our database connection.
        /// <para>Example:</para>
        /// <para>optionsBuilder.UseSqlServer(connectionString);</para>
        /// </summary>
        /// <param name="optionsBuilder">Model building options.</param>
        public abstract void DatabaseConfig(DbContextOptionsBuilder optionsBuilder);

        #endregion

        #endregion

        #region Virtual methods

        #region Ignore specific entities

        /// <summary>
        /// Virtual method to ignore specific entities from our context.
        /// <para>Example:</para>
        /// <para>modelBuilder.Ignore&lt;AspNetUsers&gt;();</para>
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder instance.</param>
        public virtual void IgnoreEntities(ModelBuilder modelBuilder)
        {
            //No ignored entities by default.
        }

        #endregion

        #region Saving exceptions

        /// <summary>
        /// Virtual method to handle exceptions on our context.
        /// <para>Examples:</para>
        /// <para>Logger.Log(ex);</para>
        /// </summary>
        /// <param name="ex">Exception class.</param>
        public virtual void SaveException(Exception ex)
        {
            throw ex;
        }

        #endregion

        #endregion

        #region OnConfiguring

        /// <summary>
        /// Abstract method to configure the database connection.
        /// </summary>
        /// <param name="optionsBuilder">Model building options.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DatabaseConfig(optionsBuilder);
        }

        #endregion

        #region OnModelCreating

        /// <summary>
        /// Abstract method to map our entities using reflection.
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder instance.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Base ModelBuilder

            try
            {
                base.OnModelCreating(modelBuilder);
            }
            catch (InvalidOperationException ex)
            {
                SaveException(ex);
                throw ex;
            }

            #endregion

            #region Ignoring specific entities

            IgnoreEntities(modelBuilder);

            #endregion

            #region Generic model building

            // Interface of our entities.
            var mappingInterface = typeof(IEntityTypeConfiguration<>);

            // Entity types to be mapped.
            var mappingTypes = typeof(TContext).GetTypeInfo()
                                               .Assembly.GetTypes()
                                               .Where(x => x.GetInterfaces()
                                               .Any(y => y.GetTypeInfo().IsGenericType &&
                                                         y.GetGenericTypeDefinition() == mappingInterface));

            // ModelBuilder's generic method.
            var entityMethod = typeof(ModelBuilder).GetMethods()
                                                   .Single(x => x.Name == "Entity" &&
                                                                x.IsGenericMethod &&
                                                                x.ReturnType.Name == "EntityTypeBuilder`1");

            foreach (var mappingType in mappingTypes)
            {
                try
                {
                    // Entity type to be mapped.
                    var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();

                    // builder.Entity<TEntity> method.
                    var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);

                    // Calling builder.Entity<TEntity> to obtain the model builder of our entity.
                    var entityBuilder = genericEntityMethod.Invoke(modelBuilder, null);

                    // Creating a new mapping instance.
                    var mapper = Activator.CreateInstance(mappingType);

                    //Invokes the "Configure" method of each entity's mapping class.
                    mapper.GetType().GetMethod("Configure")?.Invoke(mapper, new[] { entityBuilder });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debugger.Break();
                    SaveException(ex);
                }
            }

            #endregion
        }

        #endregion

        #region SetTrackingMethod

        /// <summary>
        /// Define the default tracking behavior of our context.
        /// </summary>
        /// <param name="trackingBehavior">Enum of type QueryTrackingBehavior.</param>
        public void SetTrackingBehavior(QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.NoTracking)
        {
            try
            {
                this.ChangeTracker.QueryTrackingBehavior = trackingBehavior;
            }
            catch (InvalidOperationException ex)
            {
                var error = new Exception(string.Format(Errors.Possible_0, $"map your entities. Context: ({this.GetType().Name})"), ex);
                SaveException(error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
                SaveException(ex);
            }
        }

        #endregion
    }
}
