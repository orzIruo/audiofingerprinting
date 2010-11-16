/* ------------------------------------------------------------------

   libofa -- the Open Fingerprint Architecture library

   Public Domain (PD) 2006 Predixis Corporation
   No rights reserved.

-------------------------------------------------------------------*/

#include "protocol.h"

AudioData* loadWaveFile(char *file);
AudioData* loadDataUsingLAME(char *file);

int main(int argc, char **argv) {
	AudioData *data = 0;

	// Go through each filename passed on the command line
    for (int i = 1; i < argc; ++i) {
		char *file = argv[i];

		// Get the extension
		char fext[100] = "";
		char *p = strrchr(file, '.');
		if ( p != NULL ) {
			strcpy(fext, p+1);

			// Lowercase the extension
			p = fext;
			while ( *p ) {
				*p = tolower(*p);
				p++;
			}
		}

		if ( strstr(fext, "wav")) {
			// Process a Wave file
			printf("Checking file %s\n", file);
			data = loadWaveFile(file);
		}
		else {
			// Handle anything else
			printf("Decoding file %s\n", file);
			data = loadDataUsingLAME(file);
		}
		if (!data) {
			printf("** Failed to load file\n");
			return 0;
		}

		// Get the fingerprint
		if (!data->createPrint()) {
			printf("** Failed to generate print.\n");
			return 0;
		}

		// Get the metadata
		TrackInformation *info = data->getMetadata("a7f6063296c0f1c9b75c7f511861b89b", "1.0", true);
		if (!info) {
			printf("** Failed to get metadata.\n");
			return 0;
		}
		// Print results.
		printf(" Title: %s\n", info->getTrack().c_str());
		printf("Artist: %s\n", info->getArtist().c_str());
		printf("  PUID: %s\n", info->getPUID().c_str());

		delete data;
    }
    return 0;
}


