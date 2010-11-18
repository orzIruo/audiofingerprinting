Math.NET Iridium Features Overview
Print
RSS
Modified on 2009-05-22 14:22 by ruegg
Categorized as Iridium
» Math.NET Iridium Features Overview
You can suggest or vote for new features or ideas here.

Core¶
Namespace: MathNet.Numerics

Complex Numbers (including trigonometric functions)
Quaternions (including exponential functions)

Polynomials (fast multiplication and evaluation)
Rationals

Number class, for safe floating point handling (EpsilonOf, AlmostEqual, AlmostZero, CoerceZero, Increment, Decrement)

Mathematical Constants
Scientific Constants (2007 CODATA)
Scientific Prefixes
Various helper and data structure classes

Special Functions¶
Namespace: MathNet.Numerics
Class: MathNet.Numerics.Fn

Hypot (stable sqrt(a^2+b^2))
Integer Power
Base 2 Integer Power and Logarithm
Ceiling/Floor to power of 2

Greatest common divisor (gcd), of two or more integers
Extended greatest common divisor of two numbers
Least common multiple (lcm), of two or more integers

Sinus Cardinalis (sinc)

Logarithmic Factorial
Factorial
Logarithmic Binomial Coefficient
Binomial Coefficient

Logarithmic Gamma
Gamma (supports negative numbers as well)
Regularized Gamma
Inverse Regularized Gamma
Digamma (Psi)
Logarithmic Beta
Beta
Regularized Beta
Error Function
Inverse Error Function
Harmonic Number

Trigonometry Functions¶
Namespace: MathNet.Numerics
Class: MathNet.Numerics.Trig

Conversion between degree, radian and grad.
Sine, Cosine, Tangent, Cotangent, Secant, Cosecant
Inverse: Sine, Cosine, Tangent, Cotangent, Secant, Cosecant
Hyperbolic: Sine, Cosine, Tangent, Cotangent, Secant, Cosecant
Inverse Hyperbolic: Sine, Cosine, Tangent, Cotangent, Secant, Cosecant

Combinatorics¶
Namespace: MathNet.Numerics
Class: MathNet.Numerics.Combinatorics

Counting: Variations, Variations with repetition, Combinations, Combinations with repetition, Permutations
Generate Randomly: Variations, Variations with repetition, Combinations, Combinations with repetition, Permutations
Random Shuffling (Random Permutations)
Select Random Subset: Variation, Variation with repetition, Combination, Combination with repetition

Probability Distributions¶
Namespace: MathNet.Numerics.Distributions

Continuous Probability Distributions¶

Continuous probability distributions support both the probability density function (pdf) and the cumulative distribution function (cdf), as well as the usual probability parameters. Additionally, random numbers can be generated based on the configured probability model parameters and some random number source.

All implementations inherit from the public abstract base class ContinuousDistribution and implement both interfaces IContinuousProbabilityDistribution and IContinuousGenerator.

Continuous Uniform Distribution
Triangular Distribution
Standard Distribution (Gaussian)
Normal Distribution (Gaussian with mean and variance)
Lognormal Distribution
Exponential Distribution
Laplace Distribution
Gamma Distribution
Beta Distribution
Student's T Distribution
Fisher Snedecor Distribution
Erlang Distribution
Cauchy Lorentz Distribution
Chi Distribution
ChiSquared Distribution
Pareto Distribution
Stable Distribution
Rayleigh Distribution

Discrete Probability Distributions¶

Discrete probability distributions support both the probability mass function (pmf) and the cumulative distribution function (cdf), as well as the usual probability parameters. Additionally, random numbers can be generated based on the configured probability model parameters and some random number source.

All implementations inherit from the public abstract base class DiscreteDistribution and implement both interfaces IDiscreteProbabilityDistribution and IDiscreteGenerator.

Discrete Uniform Distribution
Arbitrary Distribution
Bernoulli Distribution
Binomial Distribution
Geometric Distribution
Hypergeometric Distribution
Poisson Distribution
Zipf Distribution

Code Sample¶

StudentsTDistribution dist = new StudentsTDistribution(2);
double a = dist.Variance;
double b = dist.ProbabilityDensity(1);

Random Sources¶
Namespace: MathNet.Numerics.RandomSources

All implementations inherit the public abstract base class RandomSource.

System Random Source
Mersenne Twister Random Source
Additive Lagged Fibonacci Random Source
Xor Shift Random Source

If unsure what to choose, we recommend to simply use SystemRandomSource which internally uses the random source provided by the .Net Framework. Note that random sources should be reused, so be careful to create only one instance (per thread) and share it internally.

Code Sample¶

MersenneTwisterRandomSource src = new MersenneTwisterRandomSource();
StudentsTDistribution dist = new StudentsTDistribution(src);
double a = dist.NextDouble();

Interpolation¶
Namespace: MathNet.Numerics.Interpolation

Most interpolation algorithms also support numeric differentiation and integration. A facade class Interpolation is provided for easy access, but if needed the algorithms can also be used directly in the Algorithms sub-namespace. All implementations implement the interface IInterpolationMethod.

Rational Pole Free Interpolation, on arbitrary points (Barycentric Algorithm)
Polynomial Interpolation, on arbitrary points (Neville Algorithm)
Polynomial Interpolation, on equidistant points (Barycentric Algorithm)
Polynomial Interpolation, on first kind Chebychev points (Barycentric Algorithm)
Polynomial Interpolation, on second kind Chebychev points (Barycentric Algorithm)
Rational Interpolation, on arbitrary points (with poles; Bulirsch & Stoer Algorithm)
Linear Spline Interpolation, on arbitrary points
Cubic Spline Interpolation, with boundary conditions on arbitrary points
Natural Cubic Spline Interpolation, on arbitrary points
Akima Cubic Spline Interpolation, on arbitrary points
Custom Barycentric Interpolation, based on provided barycentric weights
Custom Spline Interpolation, based on provided spline coefficients
Custom Cubic Hermite Spline Interpolation, based on provided derivatives

If unsure what to choose, we recommend to simply use Interpolation.Create(x,y) which internally uses the barycentric rational pole free interpolation.

Code Sample¶

double[] t = new double[] { -2.0, -1.0, 0.0, 1.0, 2.0 };
double[] x = new double[] { 1.0, 2.0, -1.0, 0.0, 1.0 };
IInterpolationMethod method = Interpolation.Create(t, x);
double a = method.Interpolate(-0.5);

Linear Algebra¶
Namespace: MathNet.Numerics.LinearAlgebra

Vector, Real and Complex
Matrix, Real and Complex
LU Decomposition (Real only)
QR Decomposition (Real only)
Eigenvalue Decomposition (Real only)
Singular Value Decomposition (Real only)
Cholesky Decomposition (Real only)
Solve linear systems on a Least Square (L2) or a Least Absolute Deviation (L1) criterion
Common linear algebra operations and properties on matrices and vectors

Integral Transformations¶
Namespace: MathNet.Numerics.Transformations

Real Fast Fourier Transformation
Complex Fast Fourier Transformation

The transformation behavior can be configured (scaling, exponent sign, etc). See here for more details and code samples around fast Fourier transformations.

Code Sample¶

double[] data = new double[] { ... };
double[] freqReal, freqImag;
RealFourierTransformation rft = new RealFourierTransformation();
rft.TransformForward(data, out freqReal, out freqImag);