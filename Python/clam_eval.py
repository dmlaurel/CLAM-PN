#clam_eval.py

import csv
import numpy as np
from petrinet import PNPlace, PNTransition, PNStruct
#from numpy import genfromtxt

vox_dim = 10
#x0 = []

def evaluatePNStructure(pn, commands):
	total_vox = vox_dim * 2 - 1

	old_s = pn.getState()
	for c in commands:
		print(len(c))
		for i in range(0,total_vox): # row
			for j in range(0,total_vox): # col
				ind = total_vox * i + j
				if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
					t1 = pn.transitions[ind*4]
					t2 = pn.transitions[ind*4 + 1]
					#print(ind*4 + 2)
					t3 = pn.transitions[ind*4 + 2]
					t4 = pn.transitions[ind*4 + 3]

					#print(ind)
					if c[ind] == 1:
						pn.attemptTransition(t1.ind)
						pn.attemptTransition(t2.ind)
					else:
						pn.attemptTransition(t3.ind)
						pn.attemptTransition(t4.ind)
		s = pn.getState()
		#print(pn.getState()[0::3])
		printState(pn.getState()[0::3] - old_s[0::3])
		old_s = s
		#print(s[s < 0])
	#print(pn.getState()[0::3])


def printState(s):
	#s = pn.getState()[0::3]
	for i in range(0,19):
		o = ""
		for j in range(0,19):
			o = o + str(s[i*19 + j]) + ", "
		print(o)

def createPNStructure():
	total_vox = vox_dim * 2 - 1

	pn = PNStruct(vox_dim)
	#print(total_vox)
	#A = np.zeros((total_vox**2 * 4, total_vox**2 * 3))
	#print(A.shape)
	x0 = np.zeros(total_vox**2 * 3)
	#print(A[0:4,0:3])
	#sub_matrix_self_edge = np.asarray([[1, -3, 3], [1, -4, 4], [0, -1, 1], [0, -2, 2]])
	#because no edge exceptions rn, we can only build pyramid-like structures
	for i in range(0,total_vox): # row
		for j in range(0,total_vox): # col
			ind = total_vox * i + j
			if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
				x0[ind*3] = 0
				if (i % 2 == 1 and j % 2 == 1):
					x0[ind*3 + 1] = 0
					x0[ind*3 + 2] = 4
				else:
					x0[ind*3 + 1] = 4
					x0[ind*3 + 2] = 0	

				p1 = PNPlace(ind*3, x0[ind*3])
				p2 = PNPlace(ind*3 + 1, x0[ind*3 + 1])
				p3 = PNPlace(ind*3 + 2, x0[ind*3 + 2])

				pn.addPlaces([p1, p2, p3])

				t1 = PNTransition(ind*4)
				places = {ind*3:1, ind*3+2:4, ind*3+2:-1, ind*3+1:-3}
				t1.places = places

				t2 = PNTransition(ind*4 + 1)
				places = {ind*3:1, ind*3+2:4, ind*3+1:-4}
				t2.places = places

				t3 = PNTransition(ind*4 + 2)
				places = {ind*3+1:-1, ind*3+2:-3, ind*3+2:4}
				t3.places = places

				t4 = PNTransition(ind*4 + 3)
				places = {ind*3+1:-2, ind*3+2:-2, ind*3+2:4}
				t4.places = places

				if (i > 0 and j > 0):
					ind_sub = total_vox * (i-1) + j - 1
					t1.places[ind_sub*3 + 1] = 1
					t2.places[ind_sub*3 + 1] = 1

				if i < total_vox - 1 and j < total_vox - 1:
					ind_sub = total_vox * (i+1) + j + 1
					t1.places[ind_sub*3 + 1] = 1
					t2.places[ind_sub*3 + 1] = 1

				if i > 0 and j < total_vox - 1:
					ind_sub = total_vox * (i-1) + j + 1
					t1.places[ind_sub*3 + 1] = 1
					t2.places[ind_sub*3 + 1] = 1

				if i < total_vox - 1 and j > 0:
					ind_sub = total_vox * (i+1) + j - 1
					t1.places[ind_sub*3 + 1] = 1
					t2.places[ind_sub*3 + 1] = 1

				pn.addTransitions([t1, t2, t3, t4])
	return pn
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
	#my_data = np.genfromtxt(file_path, dtype='uint8', delimiter=",")
	#print(array)
	return array

def createPetriMatrix():

	#print(A)
	sub_matrix_self = np.asarray([[1, -3, 3], [1, -4, 4], [0, -1, 1], [0, -2, 2]])
	#sub_matrix_self = np.asarray([[1, -3, 3], [1, -4, 4], [0, -1, 1], [0, -2, 2]])

	sub_matrix_cross = np.asarray([[0, 1, -1], [0, 1, -1], [0, 0, 0], [0, 0, 0]])
	#sub_matrix_cross = np.asarray([[0, 0, 0], [0, 0, 0], [0, 0, 0], [0, 0, 0]])

	total_vox = vox_dim * 2 - 1
	#print(total_vox)
	A = np.zeros((total_vox**2 * 4, total_vox**2 * 3))
	#print(A.shape)
	x0 = np.zeros(total_vox**2 * 3)
	#print(A[0:4,0:3])
	#sub_matrix_self_edge = np.asarray([[1, -3, 3], [1, -4, 4], [0, -1, 1], [0, -2, 2]])
	#because no edge exceptions rn, we can only build pyramid-like structures
	for i in range(0,total_vox): # row
		for j in range(0,total_vox): # col
			ind = total_vox * i + j
			if (i % 2 == 0 and j % 2 == 0) or (i % 2 == 1 and j % 2 == 1): # even or odd layer
				x0[ind*3] = 0
				if (i % 2 == 1 and j % 2 == 1):
					x0[ind*3 + 1] = 0
					x0[ind*3 + 2] = 4
				else:
					x0[ind*3 + 1] = 4
					x0[ind*3 + 2] = 0	
				A[ind*4:ind*4+4,ind*3:ind*3+3] = sub_matrix_self
				if (i > 0 and j > 0):
					ind_sub = total_vox * (i-1) + j - 1
					A[ind_sub*4:ind_sub*4+4,ind*3:ind*3+3] = sub_matrix_cross
					A[ind*4:ind*4+4,ind_sub*3:ind_sub*3+3] = sub_matrix_cross
				if i < total_vox - 1 and j < total_vox - 1:
					ind_sub = total_vox * (i+1) + j + 1
					#print(A[ind_sub*4:ind_sub*4+4,ind*3:ind*3+3])
					#print(str(i) + " " + str(j) + " " + str(ind_sub*4))
					A[ind_sub*4:ind_sub*4+4,ind*3:ind*3+3] = sub_matrix_cross
					A[ind*4:ind*4+4,ind_sub*3:ind_sub*3+3] = sub_matrix_cross
				if i > 0 and j < total_vox - 1:
					ind_sub = total_vox * (i-1) + j + 1
					A[ind_sub*4:ind_sub*4+4,ind*3:ind*3+3] = sub_matrix_cross
					A[ind*4:ind*4+4,ind_sub*3:ind_sub*3+3] = sub_matrix_cross
				if i < total_vox - 1 and j > 0:
					ind_sub = total_vox * (i+1) + j - 1
					A[ind_sub*4:ind_sub*4+4,ind*3:ind*3+3] = sub_matrix_cross
					A[ind*4:ind*4+4,ind_sub*3:ind_sub*3+3] = sub_matrix_cross

			#if i == j:
			#	A[i*4:i*4+4,j*3:j*3+3] = sub_matrix_self
				#A[0:4][0:3] = sub_matrix_self
			#else:
			#	A[i*4:i*4+4,j*3:j*3+3] = sub_matrix_cross
	#print(A)
	#print(x0)
	return A, x0

commands = readVoxelObject("voxel_structure.csv")
pn = createPNStructure()
evaluatePNStructure(pn, commands)
#print(commands)
#x0 = createStartState()
#A, x0 = createPetriMatrix()
#print(x0)
#res = runVoxelCommands(x0, commands, A)
#print(res)