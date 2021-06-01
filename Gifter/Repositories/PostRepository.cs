using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Gifter.Models;
using Gifter.Utils;

namespace Gifter.Repositories
{
				public class PostRepository : BaseRepository, IPostRepository
				{
								public PostRepository(IConfiguration configuration) : base(configuration) { }

								public List<Post> GetAll()
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																								SELECT		p.Id AS PostId,
																																p.Title,
																																p.Caption,
																																p.DateCreated AS PostDateCreated,
																																p.ImageUrl AS PostImageUrl,
																																p.UserProfileId,

																																up.[Name], 
																																up.Bio, 
																																up.Email, 
																																up.DateCreated AS UserProfileDateCreated, 
																																up.ImageUrl AS UserProfileImageUrl
																								FROM Post AS p
																												LEFT JOIN UserProfile up ON p.UserProfileId = up.id
																								ORDER BY p.DateCreated
																				";

																				SqlDataReader reader = cmd.ExecuteReader();

																				List<Post> posts = new List<Post>();
																				while (reader.Read())
																				{
																								posts.Add(new Post()
																								{
																												Id = DbUtils.GetInt(reader, "PostId"),
																												Title = DbUtils.GetString(reader, "Title"),
																												Caption = DbUtils.GetString(reader, "Caption"),
																												DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
																												ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
																												UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
																												UserProfile = new UserProfile()
																												{
																																Id = DbUtils.GetInt(reader, "UserProfileId"),
																																Name = DbUtils.GetString(reader, "Name"),
																																Email = DbUtils.GetString(reader, "Email"),
																																DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
																																ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
																												}
																								});
																				}

																				reader.Close();

																				return posts;
																}

												}
								}

								public List<Post> GetAllWithComments()
								{
												//Create SQL connection
												using (SqlConnection conn = Connection)
												{
																//Connect to SQL database
																conn.Open();
																//Create the SQL Query, joining UserProfile and Comments to Post table
																using (var cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																				SELECT		p.Id AS PostId, 
																												p.Title, 
																												p.Caption, 
																												p.DateCreated AS PostDateCreated,
																												p.ImageUrl AS PostImageUrl,
																												p.UserProfileId AS PostUserProfileId,

																												up.Name,
																												up.Bio,
																												up.Email,
																												up.DateCreated AS UserProfileDateCreated,
																												up.ImageUrl AS UserProfileImageUrl,

																												c.Id AS CommentId, 
																												c.Message, 
																												c.UserProfileId AS CommentUserProfileId
																				FROM Post p
                       LEFT JOIN UserProfile up ON p.UserProfileId = up.id
                       LEFT JOIN Comment c on c.PostId = p.id
																				ORDER BY p.DateCreated";

																				SqlDataReader reader = cmd.ExecuteReader();

																				//Create a new empty list to store posts returned from database
																				List<Post> posts = new List<Post>();
																				//Initiate a while loop to iterate through database results
																				while (reader.Read())
																				{
																								//Declare postId variable to store current "PostId" from table row.
																								int postId = DbUtils.GetInt(reader, "PostId");

																								// Declare existingPost to reference the first post object in the posts list
																								// that matches the postId variable.
																								Post existingPost = posts.FirstOrDefault(p => p.Id == postId);
																								// If existing post = null, create a new Post object from DB data,
																								// which contains a UserProfile object
																								if (existingPost == null)
																								{
																												existingPost = new Post()
																												{
																																Id = postId,
																																Title = DbUtils.GetString(reader, "Title"),
																																Caption = DbUtils.GetString(reader, "Caption"),
																																DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
																																ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
																																UserProfileId = DbUtils.GetInt(reader, "PostUserProfileId"),
																																UserProfile = new UserProfile()
																																{
																																				Id = DbUtils.GetInt(reader, "PostUserProfileId"),
																																				Name = DbUtils.GetString(reader, "Name"),
																																				Email = DbUtils.GetString(reader, "Email"),
																																				DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
																																				ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
																																},
																																Comments = new List<Comment>()
																												};

																												// Add existingPost to posts list
																												posts.Add(existingPost);
																								}
																								// If the result of the IsNotDbNull method is true,
																								// after reading the CommentId value of the current DB row,
																								// create a new Comment object from table join in the SQL command
																								if (DbUtils.IsNotDbNull(reader, "CommentId"))
																								{
																												existingPost.Comments.Add(new Comment()
																												{
																																Id = DbUtils.GetInt(reader, "CommentId"),
																																Message = DbUtils.GetString(reader, "Message"),
																																PostId = postId,
																																UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
																												});
																								}
																				}

																				// Close database connection
																				reader.Close();

																				// Return the posts list
																				return posts;
																}
												}
								}

								public Post GetById(int id)
								{
												using (var conn = Connection)
												{
																conn.Open();
																using (var cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
                          SELECT				p.Title, 
																																				p.Caption, 
																																				p.DateCreated AS PostDateCreated, 
																																				p.ImageUrl AS PostImageUrl, 
																																				p.UserProfileId,

																																				up.[Name], 
																																				up.Bio, 
																																				up.Email, 
																																				up.DateCreated AS UserProfileDateCreated, 
																																				up.ImageUrl AS UserProfileImageUrl
                            FROM Post AS p
																																LEFT JOIN UserProfile up ON p.UserProfileId = up.Id
                           WHERE p.Id = @Id";

																				DbUtils.AddParameter(cmd, "@Id", id);

																				var reader = cmd.ExecuteReader();

																				Post post = null;
																				if (reader.Read())
																				{
																								post = new Post()
																								{
																												Id = id,
																												Title = DbUtils.GetString(reader, "Title"),
																												Caption = DbUtils.GetString(reader, "Caption"),
																												DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
																												ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
																												UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
																												UserProfile = new UserProfile()
																												{
																																Id = DbUtils.GetInt(reader, "PostUserProfileId"),
																																Name = DbUtils.GetString(reader, "Name"),
																																Email = DbUtils.GetString(reader, "Email"),
																																DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
																																ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
																												}
																								};
																				}

																				reader.Close();

																				return post;
																}
												}
								}

								public Post GetPostByIdWithComments(int id)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
																				SELECT		p.Title, 
																												p.Caption, 
																												p.DateCreated AS PostDateCreated, 
																												p.ImageUrl AS PostImageUrl, 
																												p.UserProfileId AS PostUserProfileId,

																												up.[Name], 
																												up.Bio, 
																												up.Email, 
																												up.DateCreated AS UserProfileDateCreated, 
																												up.ImageUrl AS UserProfileImageUrl,

																												c.Id AS CommentId, 
																												c.Message, 
																												c.UserProfileId AS CommentUserProfileId
                    FROM Post AS p
																												LEFT JOIN UserProfile up ON p.UserProfileId = up.Id
																												LEFT JOIN Comment c On c.PostId = p.id
                    WHERE p.Id = @Id

																				";
																				DbUtils.AddParameter(cmd, "@Id", id);

																				SqlDataReader reader = cmd.ExecuteReader();

																				Post post = null;
																				if (reader.Read())
																				{
																								post = new Post()
																								{
																												Id = id,
																												Title = DbUtils.GetString(reader, "Title"),
																												Caption = DbUtils.GetString(reader, "Caption"),
																												DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
																												ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
																												UserProfileId = DbUtils.GetInt(reader, "PostUserProfileId"),
																												UserProfile = new UserProfile()
																												{
																																Id = DbUtils.GetInt(reader, "PostUserProfileId"),
																																Name = DbUtils.GetString(reader, "Name"),
																																Email = DbUtils.GetString(reader, "Email"),
																																DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
																																ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
																												},
																												Comments = new List<Comment>()
																								};

																								if (DbUtils.IsNotDbNull(reader, "CommentId"))
																								{
																												post.Comments.Add(new Comment()
																												{
																																Id = DbUtils.GetInt(reader, "CommentId"),
																																Message = DbUtils.GetString(reader, "Message"),
																																PostId = id,
																																UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
																												});
																								}
																				}

																				reader.Close();
																				return post;
																}
												}
								}

								public void Add(Post post)
								{
												using (var conn = Connection)
												{
																conn.Open();
																using (var cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
                        INSERT INTO Post (Title, Caption, DateCreated, ImageUrl, UserProfileId)
                        OUTPUT INSERTED.ID
                        VALUES (@Title, @Caption, @DateCreated, @ImageUrl, @UserProfileId)";

																				DbUtils.AddParameter(cmd, "@Title", post.Title);
																				DbUtils.AddParameter(cmd, "@Caption", post.Caption);
																				DbUtils.AddParameter(cmd, "@DateCreated", post.DateCreated);
																				DbUtils.AddParameter(cmd, "@ImageUrl", post.ImageUrl);
																				DbUtils.AddParameter(cmd, "@UserProfileId", post.UserProfileId);

																				post.Id = (int)cmd.ExecuteScalar();
																}
												}
								}

								public void Update(Post post)
								{
												using (var conn = Connection)
												{
																conn.Open();
																using (var cmd = conn.CreateCommand())
																{
																				cmd.CommandText = @"
                        UPDATE Post
                           SET Title = @Title,
                               Caption = @Caption,
                               DateCreated = @DateCreated,
                               ImageUrl = @ImageUrl,
                               UserProfileId = @UserProfileId
                         WHERE Id = @Id";

																				DbUtils.AddParameter(cmd, "@Title", post.Title);
																				DbUtils.AddParameter(cmd, "@Caption", post.Caption);
																				DbUtils.AddParameter(cmd, "@DateCreated", post.DateCreated);
																				DbUtils.AddParameter(cmd, "@ImageUrl", post.ImageUrl);
																				DbUtils.AddParameter(cmd, "@UserProfileId", post.UserProfileId);
																				DbUtils.AddParameter(cmd, "@Id", post.Id);
																				;
																				cmd.ExecuteNonQuery();
																}
												}
								}

								public void Delete(int id)
								{
												using (var conn = Connection)
												{
																conn.Open();
																using (var cmd = conn.CreateCommand())
																{
																				cmd.CommandText = "DELETE FROM Post WHERE Id = @Id";
																				DbUtils.AddParameter(cmd, "@id", id);
																				cmd.ExecuteNonQuery();
																}
												}
								}

								public List<Post> SearchRecent(DateTime queryDate)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				string sql = @"
																								SELECT		p.Id AS PostId,
																																p.Title,
																																p.Caption,
																																p.DateCreated AS PostDateCreated, 
																																p.ImageUrl AS PostImageUrl, 
																																p.UserProfileId,

																																up.Name, 
																																up.Bio, 
																																up.Email, 
																																up.DateCreated AS UserProfileDateCreated, 
																																up.ImageUrl AS UserProfileImageUrl
																								FROM Post p 
																												LEFT JOIN UserProfile up ON p.UserProfileId = up.id
																								WHERE p.DateCreated >= @datePosted";

																				cmd.CommandText = sql;
																				DbUtils.AddParameter(cmd, "@datePosted", queryDate);
																				SqlDataReader reader = cmd.ExecuteReader();

																				var posts = new List<Post>();
																				while (reader.Read())
																				{
																								posts.Add(new Post()
																								{
																												Id = DbUtils.GetInt(reader, "PostId"),
																												Title = DbUtils.GetString(reader, "Title"),
																												Caption = DbUtils.GetString(reader, "Caption"),
																												DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
																												ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
																												UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
																												UserProfile = new UserProfile()
																												{
																																Id = DbUtils.GetInt(reader, "UserProfileId"),
																																Name = DbUtils.GetString(reader, "Name"),
																																Email = DbUtils.GetString(reader, "Email"),
																																DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
																																ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
																												},
																								});
																				}

																				reader.Close();

																				return posts;
																}
												}
								}

								public List<Post> Search(string criteria, bool sortDescending)
								{
												using (SqlConnection conn = Connection)
												{
																conn.Open();
																using (SqlCommand cmd = conn.CreateCommand())
																{
																				string sql = @"
																								SELECT		p.Id AS PostId,
																																p.Title,
																																p.Caption,
																																p.DateCreated AS PostDateCreated, 
																																p.ImageUrl AS PostImageUrl, 
																																p.UserProfileId,

																																up.Name, 
																																up.Bio, 
																																up.Email, 
																																up.DateCreated AS UserProfileDateCreated, 
																																up.ImageUrl AS UserProfileImageUrl
																								FROM Post p 
																												LEFT JOIN UserProfile up ON p.UserProfileId = up.id
																								WHERE p.Title LIKE @criteria OR p.Caption LIKE @criteria";

																				if (sortDescending)
																				{
																								sql += " ORDER BY p.DateCreated DESC";
																				}
																				else
																				{
																								sql += " ORDER BY p.DateCreated";
																				}

																				cmd.CommandText = sql;
																				DbUtils.AddParameter(cmd, "@criteria", $"%{criteria}%");
																				SqlDataReader reader = cmd.ExecuteReader();

																				var posts = new List<Post>();
																				while (reader.Read())
																				{
																								posts.Add(new Post()
																								{
																												Id = DbUtils.GetInt(reader, "PostId"),
																												Title = DbUtils.GetString(reader, "Title"),
																												Caption = DbUtils.GetString(reader, "Caption"),
																												DateCreated = DbUtils.GetDateTime(reader, "PostDateCreated"),
																												ImageUrl = DbUtils.GetString(reader, "PostImageUrl"),
																												UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
																												UserProfile = new UserProfile()
																												{
																																Id = DbUtils.GetInt(reader, "UserProfileId"),
																																Name = DbUtils.GetString(reader, "Name"),
																																Email = DbUtils.GetString(reader, "Email"),
																																DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
																																ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
																												},
																								});
																				}

																				reader.Close();

																				return posts;
																}
												}
								}
				}
}
