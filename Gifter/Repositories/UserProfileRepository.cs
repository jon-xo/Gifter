using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifter.Models;
using Gifter.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Gifter.Repositories
{
				public class UserProfileRepository : BaseRepository, IUserProfileRepository
				{
								public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

								public List<UserProfile> GetAll()
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								SELECT [Name],
																															Email,
																															ImageUrl,
																															Bio,
																															DateCreated
																								FROM UserProfile";

																				SqlDataReader reader = cmd.ExecuteReader();

																				List<UserProfile> users = new List<UserProfile>();

																				while (reader.Read())
																				{
																								UserProfile user = new UserProfile()
																								{
																												Id = DbUtils.GetInt(reader, "Id"),
																												Name = DbUtils.GetString(reader, "Name"),
																												Email = DbUtils.GetString(reader, "Email"),
																												DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
																								};
																								if (DbUtils.IsNotDbNull(reader, "ImageUrl"))
																								{
																												user.ImageUrl = DbUtils.GetString(reader, "ImageUrl");
																								}

																								if (DbUtils.IsNotDbNull(reader, "Bio"))
																								{
																												user.Bio = DbUtils.GetString(reader, "Bio");
																								}

																								users.Add(user);
																				}

																				reader.Close();

																				return users;

																}
												}
								}

								public UserProfile GetById(int id)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								SELECT Id,
																															[Name],
																															Email,
																															ImageUrl,
																															Bio,
																															DateCreated
																								FROM UserProfile
																								WHERE Id = @Id";
																				DbUtils.AddParameter(cmd, "@Id", id);

																				SqlDataReader reader = cmd.ExecuteReader();

																				UserProfile user = null;
																				if (reader.Read())
																				{
																								user = new UserProfile()
																								{
																												Id = id,
																												Name = DbUtils.GetString(reader, "Name"),
																												Email = DbUtils.GetString(reader, "Email"),
																												DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
																								};
																								if (DbUtils.IsNotDbNull(reader, "ImageUrl"))
																								{
																												user.ImageUrl = DbUtils.GetString(reader, "ImageUrl");
																								}

																								if (DbUtils.IsNotDbNull(reader, "Bio"))
																								{
																												user.Bio = DbUtils.GetString(reader, "Bio");
																								}
																				}

																reader.Close();

																return user;
																}
												}
								}

								public void Add(UserProfile user)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								INSERT INTO UserProfile ([Name], Email, ImageUrl, Bio, DateCreated) 
																								OUTPUT INSERTED.ID
																								VALUES (@Name, @Email, @ImageUrl, @Bio, @DateCreated)
																				";

																				DbUtils.AddParameter(cmd, "@Name", user.Name);
																				DbUtils.AddParameter(cmd, "@Email", user.Email);
																				DbUtils.AddParameter(cmd, "DateCreated", user.DateCreated);

																				if (user.Bio == null)
																				{
																								DbUtils.AddParameter(cmd, "@Bio", DBNull.Value);
																				}
																				else
																				{
																								DbUtils.AddParameter(cmd, "@Bio", user.Bio);
																				}

																				if (user.ImageUrl == null)
																				{
																								DbUtils.AddParameter(cmd, "@ImageUrl", DBNull.Value);
																				}
																				else
																				{
																								DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
																				}

																				user.Id = (int)cmd.ExecuteScalar();
																}
												}
								}

								public void Update(UserProfile user)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								UPDATE		UserProfile
																								SET					Name = @Name,
																																Email = @Email, 
																																ImageUrl = @ImageUrl, 
																																Bio = @Bio, 
																																DateCreated = @DateCreated
																								WHERE Id = @Id";

																				DbUtils.AddParameter(cmd, "@Title", user.Name);
																				DbUtils.AddParameter(cmd, "@Caption", user.Email);
																				DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);
																				DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
																				DbUtils.AddParameter(cmd, "@Id", user.Id);

																				cmd.ExecuteNonQuery();
																}
												}
								}

								public void Delete(int id)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
																				DbUtils.AddParameter(cmd, "@id", id);
																				cmd.ExecuteNonQuery();
																}
												}
								}
				}
}
