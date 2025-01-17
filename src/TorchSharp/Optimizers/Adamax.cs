// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static TorchSharp.torch;

namespace TorchSharp
{
    using Modules;

    public static partial class torch
    {
        public static partial class optim
        {

            /// <summary>
            /// Implements the Adamax algorithm (a variant of Adam based on infinity norm).
            ///
            /// It has been proposed in Adam: A Method for Stochastic Optimization.
            /// https://arxiv.org/abs/1412.6980
            /// </summary>
            /// <param name="parameters">Parameters to optimize.</param>
            /// <param name="lr ">Learning rate</param>
            /// <param name="beta1">Coefficient used for computing running averages of gradient and its square (default: 0.9)</param>
            /// <param name="beta2">Coefficient used for computing running averages of gradient and its square (default: 0.999)</param>
            /// <param name="eps">Term added to the denominator to improve numerical stability, i.e. avoid division-by-zero (default: 1e-8)</param>
            /// <param name="weight_decay">Weight decay (L2 penalty) (default: 0)</param>
            /// <returns></returns>
            public static Adamax Adamax(IEnumerable<Parameter> parameters, double lr = 0.002, double beta1 = 0.9, double beta2 = 0.999, double eps = 1e-8, double weight_decay = 0)
            {
                return new Adamax(parameters, lr, beta1, beta2, eps, weight_decay);
            }

            /// <summary>
            /// Implements the Adamax algorithm (a variant of Adam based on infinity norm).
            ///
            /// It has been proposed in Adam: A Method for Stochastic Optimization.
            /// https://arxiv.org/abs/1412.6980
            /// </summary>
            /// <param name="parameters">Parameters to optimize.</param>
            /// <param name="lr ">Learning rate</param>
            /// <param name="beta1">Coefficient used for computing running averages of gradient and its square (default: 0.9)</param>
            /// <param name="beta2">Coefficient used for computing running averages of gradient and its square (default: 0.999)</param>
            /// <param name="eps">Term added to the denominator to improve numerical stability, i.e. avoid division-by-zero (default: 1e-8)</param>
            /// <param name="weight_decay">Weight decay (L2 penalty) (default: 0)</param>
            /// <returns></returns>
            public static Adamax Adamax(IEnumerable<(string name, Parameter parameter)> parameters, double lr = 0.002, double beta1 = 0.9, double beta2 = 0.999, double eps = 1e-8, double weight_decay = 0)
            {
                return new Adamax(parameters.Select(np => np.parameter), lr, beta1, beta2, eps, weight_decay);
            }

            /// <summary>
            /// Implements the Adamax algorithm (a variant of Adam based on infinity norm).
            ///
            /// It has been proposed in Adam: A Method for Stochastic Optimization.
            /// https://arxiv.org/abs/1412.6980
            /// </summary>
            /// <param name="parameters">Parameters to optimize.</param>
            /// <param name="lr ">Learning rate</param>
            /// <param name="beta1">Coefficient used for computing running averages of gradient and its square (default: 0.9)</param>
            /// <param name="beta2">Coefficient used for computing running averages of gradient and its square (default: 0.999)</param>
            /// <param name="eps">Term added to the denominator to improve numerical stability, i.e. avoid division-by-zero (default: 1e-8)</param>
            /// <param name="weight_decay">Weight decay (L2 penalty) (default: 0)</param>
            /// <returns></returns>
            public static Adamax Adamax(IEnumerable<Adamax.ParamGroup> parameters, double lr = 0.002, double beta1 = 0.9, double beta2 = 0.999, double eps = 1e-8, double weight_decay = 0)
            {
                return new Adamax(parameters, lr, beta1, beta2, eps, weight_decay);
            }
        }
    }

    namespace Modules
    {
        using static torch.optim;

        public class Adamax : OptimizerHelper, IBetas
        {
            /// <summary>
            /// Implements Adamax algorithm (a variant of Adam based on infinity norm).
            ///
            /// It has been proposed in Adam: A Method for Stochastic Optimization.
            /// </summary>
            /// <param name="parameters">Parameters to optimize. This optimizer requires the <b>named</b> parameters collection.</param>
            /// <param name="lr ">Learning rate</param>
            /// <param name="beta1">Coefficient used for computing running averages of gradient and its square (default: 0.9)</param>
            /// <param name="beta2">Coefficient used for computing running averages of gradient and its square (default: 0.999)</param>
            /// <param name="eps">Term added to the denominator to improve numerical stability, i.e. avoid division-by-zero (default: 1e-8)</param>
            /// <param name="weight_decay">Weight decay (L2 penalty) (default: 0)</param>
            /// <returns></returns>
            public Adamax(IEnumerable<Parameter> parameters, double lr, double beta1 = 0.9, double beta2 = 0.999, double eps = 1e-8, double weight_decay = 0)
                : this(new ParamGroup[] { new ParamGroup { Parameters = parameters } }, lr, beta1, beta2, eps, weight_decay)
            {
            }

            /// <summary>
            /// Implements Adamax algorithm (a variant of Adam based on infinity norm).
            ///
            /// It has been proposed in Adam: A Method for Stochastic Optimization.
            /// </summary>
            /// <param name="parameters">Parameters to optimize. This optimizer requires the <b>named</b> parameters collection.</param>
            /// <param name="lr ">Learning rate</param>
            /// <param name="beta1">Coefficient used for computing running averages of gradient and its square (default: 0.9)</param>
            /// <param name="beta2">Coefficient used for computing running averages of gradient and its square (default: 0.999)</param>
            /// <param name="eps">Term added to the denominator to improve numerical stability, i.e. avoid division-by-zero (default: 1e-8)</param>
            /// <param name="weight_decay">Weight decay (L2 penalty) (default: 0)</param>
            /// <returns></returns>
            public Adamax(IEnumerable<ParamGroup> parameters, double lr = 0.002, double beta1 = 0.9, double beta2 = 0.999, double eps = 1e-8, double weight_decay = 0)
            {
                if (lr < 0.0) throw new ArgumentException($"Invalid learning rate: {lr}");
                if (beta1 < 0.0 || beta1 > 1.0) throw new ArgumentException($"Invalid beta1 value: {beta1}");
                if (beta2 < 0.0 || beta2 > 1.0) throw new ArgumentException($"Invalid beta2 value: {beta2}");
                if (eps < 0.0) throw new ArgumentException($"Invalid eps value: {eps}");
                if (weight_decay < 0.0) throw new ArgumentException($"Invalid weight_decay value: {weight_decay}");

                var options = new Options {
                    LearningRate = lr,
                    InitialLearningRate = lr,
                    beta1 = beta1,
                    beta2 = beta2,
                    eps = eps,
                    weight_decay = weight_decay
                };

                _defaults = options;
                _parameter_groups = new List<Modules.ParamGroup>();

                foreach (var g in parameters) {
                    add_param_group(g);
                }
            }

            /// <summary>
            /// Performs a single optimization step (parameter update).
            /// </summary>
            /// <param name="closure">A closure that reevaluates the model and returns the loss. Optional for most optimizers.</param>
            /// <returns></returns>
            public override Tensor step(Func<Tensor> closure = null)
            {
                return _step<ParamGroup>(group => {

                    var options = group.Options as Options;
                    var beta1 = options.beta1.Value;
                    var beta2 = options.beta2.Value;
                    var eps = options.eps.Value;
                    var weight_decay = options.weight_decay.Value;
                    var lr = options.LearningRate.Value;

                    foreach (var param in group.Parameters) {

                        var grad = param.grad();

                        if (grad is null) continue;

                        if (grad.is_sparse) throw new ArgumentException("Adamax does not support sparse gradients");

                        var state = _state[param.handle];

                        state.step += 1;

                        var exp_avg = state.exp_avg;
                        var exp_inf = state.exp_inf;

                        grad = (weight_decay != 0)
                            ? grad.add(param, alpha: weight_decay)
                            : grad.alias();

                        exp_avg.mul_(beta1).add_(grad, alpha: 1 - beta1);

                        var norm_buf = torch.cat(new Tensor[] {
                                        exp_inf.mul_(beta2).unsqueeze(0),
                                        grad.abs().add_(eps).unsqueeze_(0)
                                    }, 0);

                        torch.amax(norm_buf, new long[] { 0 }, false, exp_inf);

                        var clr = lr / (1 - Math.Pow(beta1, state.step));
                        param.addcdiv_(exp_avg, exp_inf, value: -clr);
                    }
                }, closure);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                foreach (var kvp in _state) {
                    kvp.Value.Dispose();
                }
            }

            private class State : IDisposable
            {
                public long step;
                public Tensor exp_avg;
                public Tensor exp_inf;

                public void Dispose()
                {
                    exp_avg.Dispose();
                    exp_inf.Dispose();
                }
            }

            /// <summary>
            /// Add a param group to the Optimizer s param_groups.
            /// </summary>
            /// <param name="param_group"></param>
            /// <remarks>This can be useful when fine tuning a pre-trained network as frozen layers can be made trainable and added to the Optimizer as training progresses.</remarks>
            public override void add_param_group(Modules.ParamGroup param_group)
            {
                var def = _defaults as Options;
                if (param_group.Options is null) {
                    param_group.Options = new Options();
                }

                var opt = param_group.Options as Options;

                // Make sure all the options are set.
                if (!opt.LearningRate.HasValue) opt.LearningRate = def.LearningRate;
                if (!opt.beta1.HasValue) opt.beta1 = def.beta1;
                if (!opt.beta2.HasValue) opt.beta2 = def.beta2;
                if (!opt.eps.HasValue) opt.eps = def.eps;
                if (!opt.weight_decay.HasValue) opt.weight_decay = def.weight_decay;

                opt.InitialLearningRate = opt.LearningRate.Value;

                _parameter_groups.Add(param_group);

                foreach (var p in param_group.Parameters) {
                    var state = new State();
                    _state[p.Handle] = state;
                    state.step = 0;
                    state.exp_avg = torch.zeros_like(p);
                    state.exp_inf = torch.zeros_like(p);
                }
            }

            public class Options : OptimizerOptions
            {
                public double? beta1;
                public double? beta2;
                public double? eps;
                public double? weight_decay;
            }

            public class ParamGroup : ParamGroup<Options>, IBetas
            {
                public ParamGroup() { }

                public ParamGroup(IEnumerable<Parameter> parameters, Options options) : base(parameters, options) { }

                public ParamGroup(IEnumerable<Parameter> parameters, double lr = 1.0, double beta1 = 0.9, double beta2 = 0.999, double eps = 1e-8, double weight_decay = 0)
                    : base(parameters, new Adamax.Options { LearningRate = lr, beta1 = beta1, beta2 = beta2, eps = eps, weight_decay = weight_decay })
                {
                }

                public (double, double) Betas {
                    get => (Options.beta1.Value, Options.beta2.Value);
                    set { Options.beta1 = value.Item1; Options.beta2 = value.Item2; }
                }
            }

            public (double, double) Betas {
                get => ((_defaults as Options).beta1.Value, (_defaults as Options).beta2.Value);
                set { (_defaults as Options).beta1 = value.Item1; (_defaults as Options).beta2 = value.Item2; }
            }

            private Dictionary<IntPtr, State> _state = new Dictionary<IntPtr, State>();
        }
    }
}
