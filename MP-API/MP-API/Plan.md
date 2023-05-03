Create a music project organizer app that can keep track of project progress

Reasoning: I have over a thousand music projects, and I would like to keep track of
which projects to prioritize working on

** Don't overcomplicate it **

- User accounts using Identity
- Database setup
- Unit of work
Project class
{
	- Id
	- Project name
	- Version Id (01a, 01b2)
	- Project owner
	- Track complete? (has the track been completed before?)
	- Who is assigned
	- A collection of specific tasks that must be completed (re-rendering, adding effects chain, etc)
}
UserProjectCollection class
{
	- Collection of all projects associated with the user (see monday.com for ideas)
}
ProjectCollectionItem class (an element of the user project collection)
{
	- Priority number
	- Project status (Planned, Active, Hiatus, Revision, Done)
	- Project
}


After the API has been configured, move on to Angular
anc create a basic app that can retrieve projects
- A user dashboard that displays overall status of projects


NOTES:
- It would be great to be able to scrape all the project titles from my file explorer