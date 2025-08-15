USE [JobApplicationTrackerDB]
GO

/****** Object:  Table [dbo].[Admin]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Admin]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[AdminId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[FullName] [nvarchar](255) NOT NULL,
	[Role] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[AdminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Admin_UserId] UNIQUE NONCLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[AdminLog]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[AdminLog]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdminLog](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[AdminId] [int] NOT NULL,
	[ActionType] [nvarchar](100) NOT NULL,
	[EntityType] [nvarchar](100) NULL,
	[EntityId] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[IpAddress] [nvarchar](45) NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
<
/****** Object:  Table [dbo].[Application]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Application]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Application](
	[ApplicationId] [int] IDENTITY(1,1) NOT NULL,
	[JobSeekerId] [int] NOT NULL,
	[JobId] [int] NOT NULL,
	[ApplicationStatusId] [int] NOT NULL,
	[AppliedAt] [datetime2](7) NOT NULL,
	[CoverLetterText] [nvarchar](max) NULL,
	[CoverLetterUrl] [nvarchar](255) NULL,
	[Feedback] [nvarchar](max) NULL,
	[LastUpdatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ApplicationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Application_JobSeekerId_JobId] UNIQUE NONCLUSTERED 
(
	[JobSeekerId] ASC,
	[JobId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ApplicationStatus]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[ApplicationStatus]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationStatus](
	[ApplicationStatusId] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ApplicationStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ApplicationStatus_Name] UNIQUE NONCLUSTERED 
(
	[StatusName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Companies]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Companies]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[WebsiteUrl] [nvarchar](255) NULL,
	[CompanyLogo] [nvarchar](255) NULL,
	[Location] [nvarchar](100) NULL,
	[ContactEmail] [nvarchar](100) NULL,
	[CreateDateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



/****** Object:  Table [dbo].[Education]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Education]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Education](
	[EducationId] [int] IDENTITY(1,1) NOT NULL,
	[School] [nvarchar](255) NOT NULL,
	[Degree] [nvarchar](255) NOT NULL,
	[FieldOfStudy] [nvarchar](255) NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[IsCurrentlyStudying] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[GPA] [decimal](3, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[EducationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Table [dbo].[Experiences]    Script Date: 8/14/2025 12:25:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Experiences](
    [ExperienceId] [int] IDENTITY(1,1) NOT NULL,
    [IsCurrentlyWorking] [bit] NOT NULL,
    [StartMonth] [int] NOT NULL,
    [StartYear] [int] NOT NULL,
    [EndMonth] [int] NULL,
    [EndYear] [int] NULL,
    [Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
    [ExperienceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
       ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, 
       OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Job]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Job]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Job](
	[JobId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[PostedByUserId] [int] NULL,

	[Title] [nvarchar](255) NOT NULL,

	[Description] [nvarchar](max) NOT NULL,
	[Location] [nvarchar](255) NULL,
	[SalaryRangeMin] [decimal](10, 2) NULL,
	[SalaryRangeMax] [decimal](10, 2) NULL,


	[JobTypeId] [int] NOT NULL,

	[EmpolymentType] [nvarchar](50) NOT NULL,


	[ExperienceLevel] [nvarchar](50) NOT NULL,
	[Responsibilities] [nvarchar](max) NULL,
	[Requirements] [nvarchar](max) NULL,
	[Benefits] [nvarchar](max) NULL,
	[PostedAt] [datetime2](7) NOT NULL,
	[ApplicationDeadline] [datetime2](7) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[Views] [int] NOT NULL,
	[EmpolymentType] [varchar](50) NULL,
	[JobType] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[JobId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[JobType]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[JobType]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobType](
	[JobTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[JobTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_JobType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Notifications]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Notifications]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[notificationId] [uniqueidentifier] NOT NULL,
	[userId] [int] NOT NULL,
	[notificationTypeId] [int] NOT NULL,
	[title] [nvarchar](255) NOT NULL,
	[message] [nvarchar](max) NOT NULL,
	[isRead] [bit] NULL,
	[createdAt] [datetime2](7) NULL,
	[linkUrl] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[notificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationTypes]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[NotificationTypes]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationTypes](
	[NotificationTypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Priority] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_NotificationType_TypeName] UNIQUE NONCLUSTERED 
(
	[TypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Skills]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Skills]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Skills](
	[SkillId] [smallint] IDENTITY(1,1) NOT NULL,
	[Skill] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Users]    Script Date: 8/14/2025 2:24:58 PM ******/

/****** Object:  Table [dbo].[Users]    Script Date: 8/15/2025 11:43:02 AM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[UserType] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[ProfilePicture] [nvarchar](255) NULL,
	[LinkedinProfile] [nvarchar](255) NULL,
	[Location] [nvarchar](100) NULL,
	[Bio] [nvarchar](max) NULL,
	[DateOfBirth] [date] NULL,
	[CompanyId] [int] NULL,
	[Skills] [varchar](50) NULL,
	[Education] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[UsersEducation]    Script Date: 8/14/2025 2:24:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersEducation](
	[EducationId] [int] IDENTITY(1,1) NOT NULL,
	[UsersId] [int] NOT NULL,
	[University] [nvarchar](255) NOT NULL,
	[College] [nvarchar](255) NOT NULL,
	[Degree] [nvarchar](100) NOT NULL,
	[FieldOfStudy] [nvarchar](100) NULL,
	[StartDate] [date] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[EndDate] [date] NULL,
	[Gpa] [decimal](3, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[EducationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsersExperience]    Script Date: 8/14/2025 2:24:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersExperience](
	[ExperienceId] [int] IDENTITY(1,1) NOT NULL,
	[UsersId] [int] NOT NULL,
	[CompanyName] [nvarchar](255) NOT NULL,
	[Position] [nvarchar](255) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ExperienceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsersSkill]    Script Date: 8/14/2025 2:24:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersSkill](
	[UsersSkillId] [int] IDENTITY(1,1) NOT NULL,
	[UsersId] [int] NOT NULL,
	[SkillId] [int] NOT NULL,
	[ProficiencyLevel] [int] NOT NULL,
	[YearsOfExperience] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UsersSkillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_JobSeekerSkill_JobSeekerId_SkillId] UNIQUE NONCLUSTERED 
(
	[UsersId] ASC,
	[SkillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AdminLog] ADD  DEFAULT (getdate()) FOR [Timestamp]
GO
ALTER TABLE [dbo].[Application] ADD  DEFAULT (getdate()) FOR [AppliedAt]
GO
ALTER TABLE [dbo].[Application] ADD  DEFAULT (getdate()) FOR [LastUpdatedAt]
GO
ALTER TABLE [dbo].[Companies] ADD  DEFAULT (getdate()) FOR [CreateDateTime]
GO
ALTER TABLE [dbo].[Education] ADD  DEFAULT ((0)) FOR [IsCurrentlyStudying]
GO
ALTER TABLE [dbo].[Job] ADD  DEFAULT (getdate()) FOR [PostedAt]
GO
ALTER TABLE [dbo].[Job] ADD  DEFAULT ('draft') FOR [Status]
GO
ALTER TABLE [dbo].[Job] ADD  DEFAULT ((0)) FOR [Views]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT (newid()) FOR [notificationId]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT ((0)) FOR [isRead]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT (getdate()) FOR [createdAt]
GO
ALTER TABLE [dbo].[NotificationTypes] ADD  DEFAULT ((1)) FOR [Priority]
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD FOREIGN KEY([notificationTypeId])
REFERENCES [dbo].[NotificationTypes] ([NotificationTypeId])
GO
