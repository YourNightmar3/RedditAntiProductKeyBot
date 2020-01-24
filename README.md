# RedditAntiProductKeyBot
A reddit bot that scans comments on specified subreddits for product keys, and once found posts a comment telling people it is not smart to post product keys on Reddit like that.

This is actually the first time i am posting something on Github so please bare with me. Also not that i am (most likely) not going to update this project anymore. Also please note that this entire project is published in one go after the development was done, so there are no commits and stuff like that in this project.

This bot was originally made by me.
My Reddit profile: https://www.reddit.com/user/YourNightmar31

# Introduction
This reddit bot was made with the intend to spread knowledge on the posting of product keys (or serial codes) on Reddit. It is generally not a good idea to do this because there are bots that can find these codes automatically and steal them.

The bot has been active for about a month until i finally shut it down after receiving a lot of criticism from moderators from big subreddits. I also didn't want my main reddit account to get in trouble because i use Reddit a lot.

The main focus of this bot was to:
  - Spread awareness of the risks of posting product keys online in the open.
  - Show people that bots can actually do a lot, by matching the pattern of the product key to what it might be used for (Steam, Uplay, Microsoft Store, etc)

I created this reddit bot with the knowledge i have from the past 3.5 years of doing my IT Software Engineering college degree, and 10+ total years of coding experience. I made sure to put as many comments as i can in the code to try and help people understand what is going on. I hope you can learn from it.

You can see the bot in action in this test thread: https://www.reddit.com/r/testingground4bots/comments/ehrr9o/testing_my_first_reddit_bot_in_the_comments/

Here is one of the latest variations of comments it posted:
https://www.reddit.com/r/testingground4bots/comments/ehrr9o/testing_my_first_reddit_bot_in_the_comments/fef0k4q?utm_source=share&utm_medium=web2x

There is also a big comment block at the top of Program.CS which contains similar info as this description does.

# The Bot
I think the bot does a good job at detecting product keys in comments. I have tried my best to keep the code as clean as possible and seperate different functionalities in different classes so beginners can use it as an example.

The code may not be *perfect* but it should be a good example for people who want to make their own reddit bot.

Here is a fun gif of the bot starting up:
![Image of the bot starting up](https://i.gyazo.com/91a34149e137b295b4ea66cee7d28b01.gif)

Some fun statistics:
- In total, this bot has scanned well over 1.5 million comments in a timespan of 24 days.
- It has posted around 50 replies not counting the ones that were for testing.
- Around 35 to 40 of those replies were posted on actual product keys. The others were false detections (mostly at the start of the development process) which i managed to minimize with lots of testing and changing the regex to detect product keys over time.

# Dependencies
I used the Newtonsoft.Json v11.02 dependency to serialize the statistics object to a JSON file, and the RedditSharp v1.1.14 library to communicate with Reddit

# Known issues
Every now and then when the reddit API returns an error, the bot throws an unhandled exception. I haven't been able to figure out where in the code it occurs. It happens maybe once a week on average.

# Disclaimer
- I am in no way responsible for what you use this code for. I published it for learning purposes. Yes i do realize it can easily be rewritten to have bad intentions.
- You are allowed to host your own anti product key bot as you please using this code. If you do, please make sure to remove my username from the comments the bot posts in CommentHandler.PostReply. I would rather not get in any more trouble with subreddit moderators.
- You are allowed to use this code as an example to create your own reddit bot. Actually, please do! That would be awesome and is exactly the reason i'm posting this.
- You are NOT ALLOWED to reupload this code and claim it was you who wrote it. That wouldn't be very nice.
- I understand that my code is not perfect, but it should be good enough to learn something from.
