# ELSA
Architecture diagram system 
![image](https://github.com/user-attachments/assets/81490dba-8e6c-492a-bcc6-b4815720bfb4)
- APIs: (.net8)
  - WebAPI: https://github.com/sz09/ELSA/tree/main/ELSA.API/ELSA.CodeChallenge.WebAPI
    - Models:
      - QuizModel: Save infomation of quiz and list of questionIds
      - QuestionModel: Save information of question
        - Provide 6 types:
          - Essay = 1, // Short text answer question
          - Binary = 2, // Yes no question
          - MultiChoice = 3, // For multi options question
          - SingleChoice = 4, // For single choice question
          - ImageSingleChoice = 5, // For multi options question but use image
          - ImageMultiChoice = 6, // For single choice question but use image
      - ScoreModel
        - Save point per user 
      - UserModel
        - Save information of user
    - Repositories: Interact with data
      - QuestionRepository
      - QuizRepository
      - ScoreRepository
      - UserRepository 
    - Services: Interact with repositories and do business
      - AnswerService
        - Answer Question: Get answer from user and decide correct or not
      - FileService
        - Store file and update the options for image type questions 
      - QuestionService
        - Almost is interact with data
        - Handling dynamic question problem due to I provide 6 types of question
      - QuizService
        - Almost is only interact with data
      - ScoreService:
        - Save score by user
        - If any change of score data, it will trigger notification service to notify connected users 
      - UserService
        - Register anonymous user or user with identity server 
      - LoggedInUserService
        - Get user information from access token if come to admin enpoints
        - Get user information from headers if come to public endpoints
    - Controllers:
      - Public: Help anonymous users can access system
      - Admin: Manage data of system
    - PrepareUserDataMiddleware:
      - Extract user's information
    - CustomModelBinderProvider
      - Handling dynamic question problem due to I provide 6 types of question
  
  - NotificationAPI: https://github.com/sz09/ELSA/tree/main/ELSA.API/ELSA.CodeChallenge.Notification
    - ELSABackgroundService
      - Watch connection into table ScoreModels
        - If any record changed (Insert / Update, Delete), it will collect top 10 scorers and notify connected users
          
  - IdentityServer: https://github.com/sz09/ELSA/tree/main/ELSA.API/ELSA.CodeChallenge.IdentityServer/ELSA.CodeChallenge.IdentityServer
    - To protect admin endpoints
      
- FrontEnd (Angular typescript)
  - SPA Application: https://github.com/sz09/ELSA/tree/main/ELSA.Web.Client
  - Components:
    - Admin
      - QuizList:
        - Show list of quizzes and provide common action: Add, Update, Delete
      - Quiz:
        - Add/ Update quiz and questions by each types
      - Questions:
        - Provide custom action and display per type of question
    - Public
      - QuizList:
        - Show list of quizzes
        - Allow user "login" by anonymous user
        - If user is not "login" yet, return to login page
      - Quiz: A cotainer only to show summarize information
      - Questions:
        - Provide custom action and display per type of question 
- Databases:
  - MongoDB: Separate database to 2 types:
    - Application database
    - IdentityServer database
- External services:
  - Azure blob storage
Features:
- Separate modules to get as small as possible module when load SPA application by module
- Localization: but for now has only English, if wanna new language, only provide a new json file
- Public page:
  - Login if not login yet
    - Provide user name and email
      - If not register by email yet, it will create  
  - AnonymousUserInterceptor:
    - Get anonymous user's information and push into headers of request if logged in when prepare request 
- Admin page: 
  - JwtInterceptor:
    - Get user's information and push into headers of request if logged in when prepare request
    - Refresh token: if still have valid refresh token, it will get new access token
