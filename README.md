\# Fitness Tracker (WPF)



Desktop application developed in \*\*C# using WPF\*\* for tracking and visualizing physical exercise performance.



The application allows users to manage exercises, register executions, and visualize training statistics through interactive charts.



This project was developed for the course \*\*Interfaces Gráficas de Usuario\*\* in the \*\*Computer Engineering Degree at the University of Salamanca\*\*.



---



\# Author



Rubén Hernández Molina  

Computer Engineering Student – University of Salamanca  



---



\# Features



The application allows users to:



• Manage exercises (add, modify, delete)  

• Register exercise executions with:

&nbsp; - repetitions

&nbsp; - weight

&nbsp; - date and time  



• Visualize execution history in tabular format  

• Visualize exercise history using \*\*bar and line charts\*\*  

• Visualize repetitions per muscle group using a \*\*radial chart\*\*



All visualizations are implemented using \*\*native WPF graphics\*\*, without external libraries.



---



\# Application Architecture



The application is structured around two main data models:



\### Ejercicio

Represents a physical exercise and contains:



\- Name

\- Description

\- Muscle groups

\- Collection of executions



The executions are stored using an `ObservableCollection` which allows automatic UI updates.



\### Ejecucion

Represents a single exercise execution and contains:



\- Repetitions

\- Weight (kg)

\- Date and time



---



\# User Interface



The application contains several windows:



\### Main Window

Displays:



\- Table with available exercises

\- Radial chart showing repetitions per muscle group for a selected date



\### Secondary Window

Displays detailed information for a selected exercise:



\- Table of executions

\- Graph showing historical exercise performance



This window is \*\*non-modal\*\*, allowing interaction with the main window simultaneously.



\### Modal Windows

Used to:



\- Add new exercises

\- Add new executions



Input validation is performed to prevent invalid data.



---



\# Data Visualization



Two types of charts are implemented:



\### Exercise History Chart



Shows:



\- repetitions using \*\*bar charts\*\*

\- weight using a \*\*line chart\*\*



Executions are grouped by date.



\### Muscle Group Chart



A \*\*radial visualization\*\* shows repetitions per muscle group for a specific date.



The values are capped at 100 repetitions as specified in the assignment requirements.



---



\# Technologies Used



\- C#

\- WPF

\- XAML

\- ObservableCollection

\- Event-based UI updates



---



\# Project Structure



src/ → source code

docs/ → assignment statement and report

images/ → screenshots of the application





---



\# Documentation



Detailed documentation of the application design and implementation can be found in:

docs/informe.pdf





---



\# Academic Context



Course: Interfaces Gráficas de Usuario  

Degree: Computer Engineering  

University: University of Salamanca

