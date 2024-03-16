# Lecturer Consulting System
This is the API of the lecturer recommendation system built using ASP.NET Core Web API C#. Our lecturer recommendation system applies two main algorithms: TOPSIS and AHP, to suggest suitable lecturers for your requirements in each subject.
## Installation Guide
1. Download this project as ZIP file or git clone: **https://github.com/longle993/LecturerConsultingSystem.git**
2. Open this project in Visual Studio and make sure you have **ASP.NET Core Web API** and **.NET 8.0** in your IDE.
3. Then, you can run and use this API.
   
## Description API
### Criteria
- ***GET /api/criteria:*** Retrieve the lecturer evaluation criteria descriptions from the system.

### Evaluation
- ***POST /api/evaluate:*** Insert a new student evaluation for a lecturer already present in the database. Make sure that your evaluation is entered in the following JSON format:

![image](https://github.com/longle993/LecturerConsultingSystem/assets/119575297/b10fe61b-291d-4ec7-bfee-41d1d848bcdb)

- **Notes:**
  - Id: The ID of the evaluation.
  - studentName: Name of the student who summitted the feedback.
  - listKansei: A list of adjectives that students have rated (the system provides pre-defined adjectives), each adjective includes the following information: id, type, name, and point.

- ***GET /api/get-all-evaluation:*** Retrieve all student feedback for a particular instructor.
### KanseiWord
- ***POST /api/kansei-word:*** Enter your requirements for the desired lecturer by scoring the adjectives we provide. The system will return a list of lecturers along with the corresponding evaluation scores that match your requirements. Make sure that the evaluation is entered in the following JSON format:

![image](https://github.com/longle993/LecturerConsultingSystem/assets/119575297/4900cf03-348e-4503-97d4-46be73074b06)

- **Notes:**
  - Each element in the array corresponds to an adjective (the system provides pre-defined adjectives to describe the desired lecturer).
  - Id: The ID of the adjective.
  - Point: Your score for the adjective describing the desired lecturer.
  - Type: The criterion that the adjective belongs to (the system has pre-assigned which adjectives belong to which criteria).

- ***GET /api/kansei-word:*** Retrieve all the adjectives used to describe lecturers.
### Teacher
- ***GET /api/teacher:*** Retrieve a subset of details of all faculty members.
