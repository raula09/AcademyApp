﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Academy.Data;

namespace Academy.Models
{
    public class StudentManagement
    {
        private readonly AcademyDbContext _context;
        private static Random _random = new Random();

        public StudentManagement(AcademyDbContext context)
        {
            _context = context;
        }

        public void AddStudent(string firstName, string lastName, DateTime dateOfBirth, string phoneNumber, string address)
        {
            try
            {
             
                string personalNumber = GeneratePersonalNumber();

                
                var student = new Student
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PersonalNumber = personalNumber,
                    DateOfEnrollment = DateTime.Now, 
                };

              
                _context.Students.Add(student);
                _context.SaveChanges();

               
                var studentDetails = new StudentDetails
                {
                    StudentId = student.Id,
                    DateOfBirth = dateOfBirth,
                    PhoneNumber = phoneNumber,
                    Address = address
                };

                _context.StudentDetails.Add(studentDetails);
                _context.SaveChanges();

                Console.WriteLine($"Student {firstName} {lastName} added successfully with ID: {student.Id}");

        
                FileManagement fileManagement = new FileManagement(_context);

               
                fileManagement.SaveAllStudentsInfoToSingleFile(student.Id);

                int courseCount = 0;

           
                while (courseCount < 4)
                {
                    Console.WriteLine("Please choose an option:");
                    Console.WriteLine("1. Add a course");
                    Console.WriteLine("2. Exit to main panel");
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Enter Course ID:");
                            int courseId = int.Parse(Console.ReadLine());

                            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
                            if (course == null)
                            {
                                Console.WriteLine("Course not found.");
                            }
                            else
                            {
                                var studentCourse = new StudentCourse
                                {
                                    StudentId = student.Id,
                                    CourseId = courseId
                                };

                                _context.StudentCourses.Add(studentCourse);
                                _context.SaveChanges();
                                Console.WriteLine($"Course {course.Name} added to student {student.FirstName} {student.LastName}.");
                                courseCount++;
                            }
                            break;

                        case "2":
                            Console.WriteLine("Exiting to the main panel.");
                            return;

                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
                var fileManagement2 = new FileManagement(_context);
                fileManagement2.LogAction($"New student added: {firstName} {lastName}, ID: {student.Id}, Personal Number: {student.PersonalNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding student: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }



        public void ViewStudents()
        {
            try
            {
                var students = _context.Students.ToList();

                if (students == null)
                {
                    Console.WriteLine("No students found.");
                    return;
                }

                foreach (var student in students)
                {
                    Console.WriteLine($"ID: {student.Id}");
                    Console.WriteLine($"Name: {student.FirstName} {student.LastName}");
                    Console.WriteLine($"Personal Number: {student.PersonalNumber}");
                    Console.WriteLine($"Date of Enrollment: {student.DateOfEnrollment.ToString("MM/dd/yyyy")}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private string GeneratePersonalNumber()
        {
            Random random = new Random();
            string r = "";

            for (int i = 0; i < 11; i++)
            {
                r += random.Next(0, 10).ToString();
            }

            return r;
        }

        public void UpdateStudent(int studentId)
        {
            var student = _context.Students.FirstOrDefault(s => s.Id == studentId);

            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }

            Console.WriteLine($"Updating details for {student.FirstName} {student.LastName}  ID: {student.Id}");
            Console.WriteLine("1. Update First Name");
            Console.WriteLine("2. Update Last Name");
            Console.WriteLine("3. Update Personal Number");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option to update: ");
            string updateChoice = Console.ReadLine();

            switch (updateChoice)
            {
                case "1":
                    Console.Write("Enter new first name: ");
                    student.FirstName = Console.ReadLine();
                    break;
                case "2":
                    Console.Write("Enter new last name: ");
                    student.LastName = Console.ReadLine();
                    break;
                case "3":
                    Console.Write("Enter new personal number: ");
                    student.PersonalNumber = Console.ReadLine();
                    break;

                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option selected.");
                    break;
            }

            _context.SaveChanges();
            Console.WriteLine("Student details updated successfully.");
        }

        public void DeleteStudent(int studentId)
        {
            try
            {
                var student = _context.Students.FirstOrDefault(s => s.Id == studentId);
                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    return;
                }

                _context.Students.Remove(student);
                _context.SaveChanges();

                Console.WriteLine($"Student ID: {studentId} deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting student: {ex.Message}");
            }
        }
    }
}
