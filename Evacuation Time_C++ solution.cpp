//Evacuation Time, C++ (g++ 4.6 std C++ 98)
#include <fstream>
#include <iostream>
#include <vector>
#include <cstring>
#include <cstdlib>
#include <string>

using namespace std;
#define MAX_TOKEN 50
#define MIN(a,b) (((a) <= (b))? (a) : (b))
#define MAX(a,b) (((a) >= (b))? (a) : (b))

vector<int> capacity;
vector< vector<int> > input_matrix;
vector<int> distribution;
vector<int> closure;
vector< vector<int> > flow;
vector<int> exit_points;
int max_path;

int compute_closure (int room) {
    vector<int> next_room;
    int i;
    int result = 0;

    for (i = 1; i < input_matrix[room].size(); i++) {
        if ((input_matrix[room][i] > 0) && (closure[i] == 0)) {
            // Node not reached yet
            closure[i] = closure[room] + 1;
            result = closure[i];
            next_room.push_back(i);
        }
    }

    for (i = 0; i < next_room.size(); i++) {
        // Compute closure for children
        int res = compute_closure(next_room[i]);
        result = MAX(result, res);
    }

    return result;
}

int move_people (int room, int & rescued) {
    vector<int> next_room;
    int i, idx;
    int result = 0;

    /* Search in original matrix to find the evacuation paths */
    for (i = 1; i < input_matrix[room].size(); i++) {
        /* Extract and order rooms based on output stream */
        if ((input_matrix[room][i] > 0) && (closure[room] < closure[i])) {
            if (next_room.empty()) {
                next_room.push_back(i);
            } else {
                vector<int>::iterator it = next_room.begin();
                for (idx = 0; idx < next_room.size(); idx++) {
                    if (input_matrix[room][i] > input_matrix[room][next_room[idx]]) {
                        break;
                    }
                }
                next_room.insert(it+idx, (int)i);
            }
        }
    }

    /* Move people closer to exit */
    for (idx = 0; idx < next_room.size(); idx++) {
        int displacement = MIN(flow[room][next_room[idx]], distribution[next_room[idx]-1]);

        if (room > 0) {
            displacement = MIN(displacement, capacity[room-1] - distribution[room-1]);
        }

        distribution[next_room[idx]-1] -= displacement;
        flow[room][next_room[idx]] -= displacement;
        result += displacement;

        if (room > 0) {
            distribution[room-1] += displacement;
        } else {
            rescued += displacement;
        }

        /* Exit if no more space */
        if (distribution[room-1] == capacity[room-1])
            break;
    }

    /* Go to next rooms */
    for (idx = 0; idx < next_room.size(); idx++) {
        if (closure[next_room[idx]] < max_path)
            result += move_people(next_room[idx], rescued);
    }

    return result;
}

    int main ( int argc, char *argv[] ){
    string inputname = argv[1];
    ifstream input_file(inputname.c_str());
    if ( argc != 2 ){
        cout << \"Expected input files are : <testno>.input ;\"<< endl;
        return 1;
    }
    if ( !input_file.is_open() ) {
        cout<<\"Could not open input file! Terminating! \" << endl;
        return 1;
    }
    else{
        char line[256];
        int N = 0;
        int result = 0;
        memset(line, 0, sizeof(line));
        input_file.getline(line, 255);
        const char* token[MAX_TOKEN] = {};
        token[0] = strtok(line, \",\");
        capacity.push_back(atoi(token[0]));
        if (token[0]){
            for (N = 1; N < MAX_TOKEN; N++){
                token[N] = strtok(0, \",\");
                if (!token[N]) break;
                capacity.push_back(atoi(token[N]));
            }
        }
        input_matrix = vector< vector<int> >(N+1, vector<int>(N+1));
        for (int i = 0; i < N+1; i++){
            input_file.getline(line, 255);
            memset (token, 0, sizeof(token));
            token[0] = strtok(line, \",\");
            input_matrix[i][0] = atoi(token[0]);
            if (token[0]){
                for (int idx = 1; idx < N+1; idx++){
                    token[idx] = strtok(0, \",\");
                    if (!token[idx]) break;
                    input_matrix[i][idx] = atoi(token[idx]);
                }
            }
        }
        /* Your code goes here */
#if 0
        cout << \"Capacity:\" << endl;
        for (int i = 0; i < N; i++) {
            cout << capacity[i] << \" \";
        }
        cout << endl;

        /* Adjust impossible flows */
        cout << endl << \"Input matrix: \" << endl;
        for (int i = 0; i < N+1; i++) {
            for (int idx = 0; idx < N+1; idx++) {
                #if 0
                int min = MIN(capacity[i-1], capacity[idx-1]);

                if (input_matrix[i][idx] > 0) {
                    if (input_matrix[i][idx] > min) {
                    // Limit the maximum flow to room capacity
                    input_matrix[i][idx] = min;
                    input_matrix[idx][i] = min;
                    }

                    if (i == 0) {
                        /* Exit point */
                        exit_points.push_back(idx);
                    }
                }
                #endif

                cout << input_matrix[i][idx] << \" \";
            }
            cout << endl;
        }
#endif

        /* Compute closure to exit points */
        closure = vector<int>(N+1);
        max_path = compute_closure(0);

#if 0
        cout << endl << \"Closure: \";
        for (int idx = 1; idx < closure.size(); idx++) {
            cout << closure[idx] << \" \";
        }
        cout << endl << \"Max_path: \" << max_path << endl;
#endif 

        /* Start with maximum distribution */
        distribution = capacity;

        /* Iterations */
        bool evacuated = false;
        int rescued = 0;
        do {
            result++;

            /* Start with a fresh transition matrix */
            flow = input_matrix;

            do {
                /* Move people until the evacuation capacity reached for each edge or rooms are full. */
            } while (move_people(0, rescued) > 0);

            evacuated = true;
            for (int idx = 0; idx < N; idx++) {
                if (distribution[idx] != 0) {
                    evacuated  = false;
                }
            }
        } while(!evacuated);

        cout << result << endl;

    }
    input_file.close();
    return 0;
}