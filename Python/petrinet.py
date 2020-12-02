#pn-node

import numpy as np

class PNPlace:
  def __init__(self, ind, marks):
    #self.transitions = {}#
    self.ind = ind
    self.marks = marks

class PNTransition:
	def __init__(self, ind):
		self.ind = ind
		self.places = {}# dict with place object index : int, where int is weight and direction of transition. from transition is positive, to is negative

class PNStruct:
	#size is length of top level places, so in our case X*Y/2
	def __init__(self, size):
		self.transitions = {}#dict of transition objects that make up PN
		self.places = {}#dict of place objects that make up PN
		self.size = size#10 if even len is 10 for example
		#self.transition_string_map = {}#dict mapping strings to their 

	def addTransitions(self, pn_t):
		for t in pn_t:
			self.transitions[t.ind] = t

	def addPlaces(self, pn_p):
		for p in pn_p:
			self.places[p.ind] = p

	def getState(self):
		total_vox = self.size * 2 - 1
		state = np.zeros(total_vox**2 * 3)

		for i in range(0,total_vox): # row
			for j in range(0,total_vox): # col
				ind = (total_vox) * i + j
				if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
					state[ind*3] = self.places[ind*3].marks
					state[ind*3 + 1] = self.places[ind*3 + 1].marks
					state[ind*3 + 2] = self.places[ind*3 + 2].marks

		return state

	def attemptTransition(self, index):
		fire = True
		for key in self.transitions[index].places:
			if (self.places[key].marks < self.transitions[index].places[key] * -1):
				fire = False

		if fire:
			for key in self.transitions[index].places:
				self.places[key].marks += self.transitions[index].places[key]

