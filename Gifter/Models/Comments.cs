﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Gifter.Models
{
				public class Comments
				{
								public int Id { get; set; }
								public string Message { get; set; }
								public int UserProfileId { get; set; }
								public UserProfile UserProfile { get; set; }
								public int PostId { get; set; }
								public Post Post { get; set; }
				}
}