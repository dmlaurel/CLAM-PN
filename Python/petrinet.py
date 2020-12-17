#pn-node

import numpy as np

class PNPlace:
  def __init__(self, event, ind, marks): # event is the location command, i.e. x*19 + y. ind is the depth, i.e. the first, second, third etc. for that event
    #self.transitions = {}#
    self.ind = ind
    self.event = event
    self.marks = marks

class PNTransition:
	def __init__(self, event, ind, mask):
		self.ind = ind
		self.event = event
		self.mask = mask#string or char that the transition is mapped to
		self.places = {}# dict with place object index : int, where int is weight and direction of transition. from transition is positive, to is negative

	def addPlace(self, h, weight):
		#print("add " + str(h) + str(type(h)))
		#for key in 
		if h in self.places.keys():
			self.places[h].append(weight)
		else:
			self.places[h] = [weight]

class PNStruct:
	#size is length of top level places, so in our case X*Y/2
	def __init__(self, size):
		self.transitions = {}#dict of transition objects that make up PN
		self.places = {}#dict of place objects that make up PN
		self.size = size#10 if even len is 10 for example
		self.transition_per_event = 0
		#self.transition_string_map = {}#dict mapping strings to their 

	def addTransitions(self, pn_t):
		for t in pn_t:
			self.transitions[self.getHash(t.event, t.ind)] = t

	def getHash(self, event, ind):
		return event + ind * 0.01 + 0.01

	def addPlaces(self, pn_p):
		for p in pn_p:
			#print("add p " + str(self.getHash(p.event, p.ind)))
			self.places[self.getHash(p.event, p.ind)] = p

	def getState(self):
		total_vox = self.size * 2 - 1
		state = np.zeros(total_vox**2 * 3)

		for i in range(0,total_vox): # row
			for j in range(0,total_vox): # col
				ind = (total_vox) * i + j
				if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
					state[ind*3] = self.places[self.getHash(ind, 0)].marks
					state[ind*3 + 1] = self.places[self.getHash(ind, 1)].marks
					state[ind*3 + 2] = self.places[self.getHash(ind, 2)].marks

		return state

	def checkTransition(self, tr, mask):
		event = tr.event
		ind = tr.ind
		fire = True
		if not (mask == tr.mask):
			return False
		h = self.getHash(event, ind)
		for key in self.transitions[h].places:

			for i in range(0,len(self.transitions[h].places[key])):
				w = self.transitions[h].places[key][i]
				if (w != "r"):
					w = int(w)

					if (self.places[key].marks < w * -1):
						fire = False
		return fire

	def fireTransition(self, tr, mask):
		event = tr.event
		ind = tr.ind

		h = self.getHash(event, ind)

			#print("fired " + str(event) + " " + str(ind))
		for key in self.transitions[h].places:
			for w in self.transitions[h].places[key]:
				if w == "r":
					self.places[key].marks = 0
				else:
					w = int(w)
					self.places[key].marks += w
