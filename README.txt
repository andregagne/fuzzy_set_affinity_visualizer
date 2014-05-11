This is the fuzzy set affinnity visualizer.

The idea behind the fuzzy set affinity visualizer is that data elements can often be described as being partially part of several sets (where people spend their time, for example).
What the visualizer does is visually represent these data points in such a way as to give viewers an overall view of the patterns in the data (e.g., where do most people spend most of their time?).

The project comes in two parts.

The first part is a generator for generating test data.  The generator is exceedingly simple so go ahead and take a look if you're curious.  

The second part is the visualizeritself, it takes in a tab delineated text file with the first row being a header (a name column and then N number of names of sets.  It expects the first column to be a data point’s name and subsequent columns to be the membership percent (as a whole number between 0 and 100) in the corresponding set.
As it deals with fuzzy sets, it does not require that the sum of the percentages for all of the sets be equal to 100

For example:
Name	setA	setB	setC
One	50	24	25
Two	3	0	97
Three	40	20	60

The core of the visualization is when multiple sets are grouped together.  When this happens, the sets are arranged evenly around a circle and the individual datapoints are arranged according to their affinity to the various sets.
The visualization takes inspiration from the dust and magnet visualization which can be found here:  http://www.cc.gatech.edu/gvu/ii/dnm/

Give the system a try!