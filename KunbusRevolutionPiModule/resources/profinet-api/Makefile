IDIR =.
CC=arm-linux-gnueabihf-gcc
#CC=gcc
CFLAGS=-I.

ODIR = obj

_DEPS = piControl.h piControlIf.h
DEPS = $(patsubst %,$(IDIR)/%,$(_DEPS))

_OBJ = piControlIf.o
OBJ = $(patsubst %,$(ODIR)/%,$(_OBJ))

OUT_LIB = $(ODIR)/libkunbuspn.so

all: $(OUT_LIB)

$(OUT_LIB): $(ODIR)/piControlIf.o
	$(CC) -shared -fPIC -Wl,-soname,libkunbuspn.so.1 -o $@ $<

$(ODIR)/%.o: %.c $(DEPS)
	$(CC) -fPIC -c -o $@ $< $(CFLAGS)
	

piTe.PHONY: clean

clean:
	rm -f $(ODIR)/*.o
