This is the fuzzy set visualizer.

The idea behind the fuzzy set visualizer is that data elements can often be described as being partially part of several sets (where people spend their time, for example).
What the visualizer does is visually represent these data points in such a way as to give viewers an overall view of the patterns in the data (e.g., where do most people spend most of their time?).

The project comes in two parts.

The first part is a generator for generating test data.  The generator is exceedingly simple so go ahead and take a look if you're curious.  

The second part is the generator itself, it takes in a tab delineated text file with the first row being a header (a name column and then N number of names of sets.  It expects the first column to be a data point’s name and subsequent columns to be the membership percent (as a whole number between 0 and 100) in the corresponding set.

For example:
Name	setA	setB	setC
One	50	24	25
Two	3	0	97
Three	20	20	60

The visualizer then presents the different sets with some set of how many members they have.  The user can drag and drop sets together to get at the heart of the visualization.  Give it a try!
