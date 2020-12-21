#Duncan Abbot, 2020

import csv
import numpy as np
from petrinet import PNPlace, PNTransition, PNStruct

vox_dim = 10

def evaluatePNStructure(pn, commands):
	total_vox = vox_dim * 2 - 1

	old_s = pn.getState()
	for c in commands:

		for i in range(0,total_vox): # row
			for j in range(0,total_vox): # col
				ind = total_vox * i + j
				if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
				
					if c[ind] == 1:
						f1 = f1 + 1
						fireString("ccccassss", pn, ind)

					elif c[ind] == 0:
						fireString("n", pn, ind)

		s = pn.getState()
		
		printState(pn.getState()[0::3] - old_s[0::3])
		exportLayer("voxel_structure_out.csv", pn.getState()[0::3] - old_s[0::3])
		old_s = s

def fireString(s, pn, ind):
	for c in s:
		#print(c)
		available_transitions = []
		for n in range(0, pn.transition_per_event):
			t = pn.transitions[pn.getHash(ind, n)]
			if pn.checkTransition(t, c):
				available_transitions.append(t)

		for n in range(0, len(available_transitions)):
			pn.fireTransition(available_transitions[n], c)


def printState(s):
	#s = pn.getState()[0::3]
	for i in range(0,19):
		o = ""
		for j in range(0,19):
			o = o + str(s[i*19 + j]) + ", "
		print(o)



def readPNConfig(file_path):

	
	total_vox = 0
	pn = PNStruct(1)
	x0 = np.zeros(1)

	f = open(file_path, "r")
	array = []
	line = f.readline()
	index = 0

	num_x = 0
	num_y = 0
	places_count = 0
	transitions_count = 0

	while line:
		line = line.strip("\n")
		line = line.replace(' ', '')

		if '#' in line:
			line = line[0: line.index('#')]

		l = line.split('/')

		if len(line) > 0 and len(line[0]) > 0 and not line[0][0] == '#':
			#print(line)
			#process lines
			#for l in line:
			#l = line
			if l[0] == "h":
				if l[1] == "vox_dim":
					vox_dim = int(l[2])
					pn = PNStruct(vox_dim)
					total_vox = vox_dim * 2 - 1
					#x0 = np.zeros(total_vox**2 * 3)
				#if l[1] == "num_y":
				#	num_y = int(l[2])
				if l[1] == "places_count":
					places_count = int(l[2])
				if l[1] == "transitions_count":
					transitions_count = int(l[2])
					pn.transition_per_event = transitions_count

			if l[0] == "p":
				for i in range(0,total_vox): # row
					for j in range(0,total_vox): # col
						event = total_vox * i + j
						if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
							#for n in range(0,transitions_count):
							#t1 = PNTransition(event, int(l[2]), l[1])
							places = []
							if (l[1] == '0' and (i % 2 == 0 and j % 2 == 0)):
								for n in range(2, len(l)):

									place_pair = l[n].split(':')
									p = PNPlace(event, int(place_pair[0]), int(place_pair[1]))
									places.append(p)

							if (l[1] == '1' and (i % 2 == 1 and j % 2 == 1)):
								for n in range(2, len(l)):
									place_pair = l[n].split(':')
									p = PNPlace(event, int(place_pair[0]), int(place_pair[1]))
									places.append(p)

							pn.addPlaces(places)
								


			if l[0] == "t":
				for i in range(0,total_vox): # row
					for j in range(0,total_vox): # col
						event = total_vox * i + j
						if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
							if (not isCorner(i,j,total_vox)):
								#for n in range(0,transitions_count):
								t1 = PNTransition(event, int(l[2]), l[1])
								for n in range(0, len(l) - 3):
									l[n+3] = l[n+3].replace(']','')
									l[n+3] = l[n+3].replace('[','')
									l_sub = l[n+3].split(':')
									p_ind = l_sub[0].split(',')
									if (i + int(p_ind[0])) > 0 and (i + int(p_ind[0])) < total_vox and (int(p_ind[1]) + j) > 0 and (int(p_ind[1]) + j) < total_vox:
										new_event = total_vox * (i + int(p_ind[0])) + int(p_ind[1]) + j
										t1.addPlace(pn.getHash(new_event, int(p_ind[2])), l_sub[1])
								pn.addTransitions([t1])

			if l[0] == "ct":
				for i in range(0,total_vox): # row
					for j in range(0,total_vox): # col
						event = total_vox * i + j
						if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
							if (isCorner(i,j,total_vox)):
								#print(event)
							#for n in range(0,transitions_count):
								t1 = PNTransition(event, int(l[2]), l[1])
								for n in range(0, len(l) - 3):
									l[n+3] = l[n+3].replace(']','')
									l[n+3] = l[n+3].replace('[','')
									l_sub = l[n+3].split(':')
									p_ind = l_sub[0].split(',')
									if (i + int(p_ind[0])) > 0 and (i + int(p_ind[0])) < total_vox and (int(p_ind[1]) + j) > 0 and (int(p_ind[1]) + j) < total_vox:
										new_event = total_vox * (i + int(p_ind[0])) + int(p_ind[1]) + j
										t1.addPlace(pn.getHash(new_event, int(p_ind[2])), l_sub[1])
								pn.addTransitions([t1])
					

		line = f.readline()
		index += 1
	f.close()

	return pn

def isCorner(i,j,total_vox):
	if (i == 0 and j == 0): return True
	if (i == 0 and j == total_vox - 1): return True
	if (i == total_vox - 1 and j == 0): return True
	if (i == total_vox -1 and j == total_vox - 1): return True
	return False

def exportLayer(file_path, layer):
	f = open(file_path, "a")
	s_layer = ""
	for l in layer:
		l_s = str(l)
		l_s = l_s.split('.')
		l_s = l_s[0]
		s_layer = s_layer + str(l_s) + ","
	s_layer = s_layer[0:len(s_layer)-1]
	s_layer += "\n"
	f.write(s_layer)
	f.close()

#returns an array with the correct voxel commands
def readVoxelObject(file_path):

	f = open(file_path, "r")
	array = []
	line = f.readline()
	index = 0
	while line:
		line = line.strip("\n")
		#line = line.strip(' ')
		line = line.split(',')
		array.append([])
		for item in line:
			#print(item)
			array[index].append(int(item))
		line = f.readline()
		index += 1
	f.close()

	return array


commands = readVoxelObject("voxel_structure.csv")
pn = readPNConfig("pn_2b4s.pn")
evaluatePNStructure(pn, commands)
